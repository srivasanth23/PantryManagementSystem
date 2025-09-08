using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.DTO;
using PantryManagementSystem.Repositories.Interfaces;

namespace PantryManagementSystem.Controllers
{
    //[Authorize(Roles = "Admin,Staff")]
    public class PantryItemController : Controller
    {
        private readonly IPantryItemRepository _repository;

        public PantryItemController(IPantryItemRepository repository)
        {
            _repository = repository;
        }

        // GET: PantryItem
        public async Task<IActionResult> Index()
        {
            var items = await _repository.GetAllAsync();
            var dtos = items.Select(i => new PantryItemReadDTO
            {
                Id = i.Id,
                Name = i.Name,
                Category = i.Category,
                Price = i.Price,
                Quantity = i.Quantity,
                ExpiryDate = i.ExpiryDate
            }).ToList();

            return View(dtos);
        }

        // GET: PantryItem/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();

            var dto = new PantryItemReadDTO
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                Price = item.Price,
                Quantity = item.Quantity,
                ExpiryDate = item.ExpiryDate
            };

            return View(dto);
        }

        // GET: PantryItem/Create
        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories();
            return View();
        }

        // POST: PantryItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PantryItemCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = GetCategories();
                return View(dto);
            }

            await _repository.AddAsync(new PantryManagementSystem.Models.Domain.PantryItem
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Category = dto.Category,
                Price = dto.Price,
                Quantity = dto.Quantity,
                ExpiryDate = dto.ExpiryDate
            });

            return RedirectToAction("Index");
        }

        // GET: PantryItem/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();

            ViewBag.Categories = GetCategories();

            var dto = new PantryItemUpdateDTO
            {
                Name = item.Name,
                Category = item.Category,
                Price = item.Price,
                Quantity = item.Quantity,
                ExpiryDate = item.ExpiryDate
            };

            return View(dto);
        }

        // POST: PantryItem/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PantryItemUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = GetCategories();
                return View(dto);
            }

            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();

            item.Name = dto.Name;
            item.Category = dto.Category;
            item.Price = dto.Price;
            item.Quantity = dto.Quantity;
            item.ExpiryDate = dto.ExpiryDate;

            await _repository.UpdateAsync(item);
            return RedirectToAction("Index");
        }
        // GET: PantryItem/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();

            await _repository.DeleteAsync(id);
            return Ok();
        }


        // Helper method: predefined categories
        private List<string> GetCategories()
        {
            return new List<string>
        {
            "Beverage","Snacks","Biscuits","Juices","Dairy","Fruits",
            "Vegetables","Bread","Condiments","Cereals","Chocolate",
            "Coffee","Tea","Nuts","Soups","Sauces","Frozen","ReadyToEat","Miscellaneous"
        };
        }
    }

}
