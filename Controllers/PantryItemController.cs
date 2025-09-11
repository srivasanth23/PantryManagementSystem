using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.DTO;
using PantryManagementSystem.Repositories.Interfaces;

namespace PantryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff,User")]
    public class PantryItemController : Controller
    {
        private readonly IPantryItemRepository _repository;

        public PantryItemController(IPantryItemRepository repository)
        {
            _repository = repository;
        }

        // GET: PantryItem (with optional search)
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var items = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                items = items.Where(i =>
                    i.Name.ToLower().Contains(searchTerm) ||
                    i.Category.ToLower().Contains(searchTerm)
                ).ToList();
            }

            var dtos = items.Select(i => new PantryItemReadDTO
            {
                Id = i.Id,
                Name = i.Name,
                Category = i.Category,
                Price = i.Price,
                Quantity = i.Quantity,
                ExpiryDate = i.ExpiryDate
            }).ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(dtos);
        }

        // GET: PantryItem/Details/{id}
        [HttpGet]
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
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories();
            return View();
        }

        // POST: PantryItem/Create
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PantryItemCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = GetCategories();
                return View(dto);
            }

            await _repository.AddAsync(new PantryItem
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
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
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
        [Authorize(Roles = "Admin,Staff")]
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
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();

            await _repository.DeleteAsync(id);
            return Ok();
        }

        public async Task<IActionResult> SearchFunctinallity(string searchTerm)
        {
            var items = await _repository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(x =>
                    x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    x.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            return View(items);
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
