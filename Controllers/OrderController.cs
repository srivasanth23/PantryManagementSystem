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
        private readonly IOrderRepository _repository;                                      // Repository to interact with the orders table
        private readonly UserManager<IdentityUser> _userManager;                            // Interacts with the UserManager

        public OrderController(IOrderRepository repository, UserManager<IdentityUser> userManager)                  // Injects the repository and the user manager 
        {
            _repository = repository;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Browse()                                           // Browse Pantry Items
        {
            var items = await _repository.BrowseAsync();                                    // Fetch all the pantry items from the repo
            return View(items);                                                             // Browse.cshtml
        }

        [HttpPost]                                                                          // Request an Item
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestItem(Guid pantryItemId, int quantity)
        {
            // Get current logged-in UserId (from Claims)
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return BadRequest("Invalid User");

            // Call repository to create a new Order
            var order = await _repository.RequestItemAsync(userId, pantryItemId, quantity);

            // If order failed, show an error 
            if (order == null)
            {
                TempData["Error"] = "Cannot request item. Check quantity or availability.";
                return RedirectToAction("UserOrdersHistory");
            }

            // Optional: automatically issue the item
            // await _repository.IssueOrderAsync(order.Id);

            TempData["Success"] = "Item requested successfully!";
            return RedirectToAction("UserOrdersHistory");                                   // Redirect user to their Order History
        }

        // GET: Request Order (form with details)
        [HttpGet]
        public async Task<IActionResult> RequestOrder(Guid id)
        {
            var item = await _repository.BrowseAsync();                                     // Fetch all the pantry items from the repo
            var pantryItem = item.FirstOrDefault(p => p.Id == id);                          // Find the specific item requested

            if (pantryItem == null)
                return NotFound();

            var dto = new RequestOrderDTO                                                   // Create DTO (view model) to send to RequestOrder.cshtml
            {
                Id = pantryItem.Id,
                Name = pantryItem.Name,
                Price = pantryItem.Price,
                AvailableQuantity = pantryItem.Quantity,
                ExpiryDate = pantryItem.ExpiryDate
            };

            return View(dto);                                                               // RequestOrder.cshtml
        }

        // POST: When user submits the Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestOrder(RequestOrderDTO model)
        {
            // Validate userId from claims
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return BadRequest("Invalid User");

            // Enter the positive quantity
            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than 0");
                return View(model);
            }

            // Place order 
            var order = await _repository.RequestItemAsync(userId, model.Id, model.Quantity);
            if (order == null)
            {
                TempData["Error"] = "Cannot request item. Check quantity or availability.";
                return RedirectToAction("Browse");
            }

            TempData["Success"] = "Item requested successfully!";

            // Redirect based on approval status
            if (order.Status == Models.Enums.OrderStatus.Approved)
                return RedirectToAction("ApprovedOrders");

            return RedirectToAction("UserOrdersHistory");                                           // Otherwise → go to user’s history
        }

        // User: Approved Orders page
        public async Task<IActionResult> ApprovedOrders()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return Unauthorized("Invalid User");

            // Fetch only approved orders for this user
            var approvedOrders = await _repository.GetApprovedOrdersAsync(userId);

            // Map Entities -> DTOs
            var dtos = approvedOrders.Select(o => new MyOrderDTO
            {
                Id = o.Id,
                PantryItemName = o.PantryItem.Name,
                Quantity = o.Quantity,
                Status = o.Status.ToString(),
                RequestDate = o.RequestDate,
                IssuedDate = o.IssuedDate
            }).ToList();

            return View(dtos);                                                                      // ApprovedOrders.cshtml
        }


        // My Orders page - Based on ROLE
        public async Task<IActionResult> MyOrders()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return Unauthorized("Invalid User");

            var orders = new List<MyOrderDTO>();

            // Staff/Admin: get ALL orders
            if (User.IsInRole("Staff") || User.IsInRole("Admin"))
            {
                // Staff/Admin: see all orders
                var allOrders = await _repository.GetAllOrdersForStaffAsync();
                orders = allOrders.ToList();
            }
            else
            {
                // Regular User: see only their own orders - Normal User: only their own orders
                var myOrders = await _repository.GetMyOrdersAsync(userId);
                orders = myOrders.Select(o => new MyOrderDTO
                {
                    Id = o.Id,
                    PantryItemName = o.PantryItem.Name,
                    Quantity = o.Quantity,
                    Status = o.Status.ToString(),
                    RequestDate = o.RequestDate,
                    IssuedDate = o.IssuedDate
                }).ToList();
            }

            return View(orders);
        }


        // Staff: Manage Orders
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ManageOrders()
        {
            var orders = await _repository.GetAllOrdersForStaffAsync();
            return View(orders); // ManageOrders.cshtml
        }

        // Admin/Staff: All Orders with Email
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _repository.GetAllOrdersAsync();
            return View(orders); // AllOrders.cshtml
        }

        // Approve Order
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

        // Deny Order
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

        // Issue Order
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

        public async Task<IActionResult> UserOrdersHistory()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return Unauthorized("Invalid User");

            // Fetch all orders for the logged-in user
            var myOrders = await _repository.GetMyOrdersAsync(userId);

            // Map to DTO list
            var dtos = myOrders.Select(o => new MyOrderDTO
            {
                Id = o.Id,
                PantryItemName = o.PantryItem.Name,
                Quantity = o.Quantity,
                Status = o.Status.ToString(),
                RequestDate = o.RequestDate,
                IssuedDate = o.IssuedDate
            }).ToList();

            return View(dtos);                                                                      // UserOrdersHistory.cshtml
        }
    }
}