using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PantryManagementSystem.Models.DTO;
using PantryManagementSystem.Models.Enums;
using PantryManagementSystem.Repositories.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PantryManagementSystem.Controllers
{
    //[Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(IOrderRepository repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // --------------------
        // Browse Pantry Items
        // --------------------
        [HttpGet]
        public async Task<IActionResult> Browse()
        {
            var items = await _repository.BrowseAsync();
            return View(items); // Browse.cshtml
        }

        // --------------------
        // Request an Item
        // --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestItem(Guid pantryItemId, int quantity)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return BadRequest("Invalid User");

            var order = await _repository.RequestItemAsync(userId, pantryItemId, quantity);

            if (order == null)
            {
                TempData["Error"] = "Cannot request item. Check quantity or availability.";
                return RedirectToAction("Browse");
            }

            // Optional: automatically issue the item
            // await _repository.IssueOrderAsync(order.Id);

            TempData["Success"] = "Item requested successfully!";
            return RedirectToAction("MyOrders");
        }

        // GET: RequestOrder page
        [HttpGet]
        public async Task<IActionResult> RequestOrder(Guid id)
        {
            var item = await _repository.BrowseAsync();
            var pantryItem = item.FirstOrDefault(p => p.Id == id);

            if (pantryItem == null)
                return NotFound();

            // Create DTO to pass to view
            var dto = new RequestOrderDTO
            {
                Id = pantryItem.Id,
                Name = pantryItem.Name,
                Price = pantryItem.Price,
                AvailableQuantity = pantryItem.Quantity,
                ExpiryDate = pantryItem.ExpiryDate
            };

            return View(dto); // RequestOrder.cshtml
        }

        // POST: Submit the order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestOrder(RequestOrderDTO model)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return BadRequest("Invalid User");

            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than 0");
                return View(model);
            }

            var order = await _repository.RequestItemAsync(userId, model.Id, model.Quantity);
            if (order == null)
            {
                TempData["Error"] = "Cannot request item. Check quantity or availability.";
                return RedirectToAction("Browse");
            }

            TempData["Success"] = "Item requested successfully!";
            return RedirectToAction("UserOrdersHistory");
        }


        // --------------------
        // My Orders page
        // --------------------
        public async Task<IActionResult> MyOrders()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return Unauthorized("Invalid User");

            var orders = await _repository.GetMyOrdersAsync(userId);

            var dtos = orders.Select(o => new MyOrderDTO
            {
                Id = o.Id,
                PantryItemName = o.PantryItem.Name,
                Quantity = o.Quantity,
                Status = o.Status.ToString(),
                RequestDate = o.RequestDate,
                IssuedDate = o.IssuedDate
            }).ToList();

            return View(dtos); // MyOrders.cshtml
        }

        // --------------------
        // Staff: Manage Orders
        // --------------------
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ManageOrders()
        {
            var orders = await _repository.GetAllOrdersForStaffAsync();
            return View(orders); // ManageOrders.cshtml
        }

        // --------------------
        // Admin/Staff: All Orders with Email
        // --------------------
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _repository.GetAllOrdersAsync();
            return View(orders); // AllOrders.cshtml
        }

        // --------------------
        // Approve Order
        // --------------------
        //[Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid orderId)
        {
            var order = await _repository.ApproveOrderAsync(orderId);
            if (order == null) return BadRequest("Order not found");

            TempData["Success"] = "Order approved successfully!";
            return RedirectToAction("AllOrders");
        }

        // --------------------
        // Deny Order
        // --------------------
        //[Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(Guid orderId)
        {
            var order = await _repository.DenyOrderAsync(orderId);
            if (order == null) return BadRequest("Order not found");

            TempData["Success"] = "Order denied!";
            return RedirectToAction("AllOrders");
        }

        // --------------------
        // Issue Order
        // --------------------
        //[Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(Guid orderId)
        {
            var order = await _repository.IssueOrderAsync(orderId);
            if (order == null)
                return BadRequest("Order cannot be issued");

            TempData["Success"] = "Order issued successfully!";
            return RedirectToAction("AllOrders");
        }
    }
}
