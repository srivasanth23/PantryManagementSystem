using Microsoft.AspNetCore.Mvc;
using PantryManagementSystem.Models.ViewModels;
using PantryManagementSystem.Repositories.Interfaces;

namespace PantryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPantryItemRepository _pantryRepo;
        //private readonly IOrderRepository _orderRepo;       // TODO: implement or inject
        //private readonly IBillingRepository _billingRepo;   // TODO: implement or inject
        //private readonly IReportRepository _reportRepo;     // TODO: implement or inject

        public HomeController(
            IPantryItemRepository pantryRepo
        //, IOrderRepository orderRepo
        //, IBillingRepository billingRepo
        //, IReportRepository reportRepo
        )
        {
            _pantryRepo = pantryRepo;
            //_orderRepo = orderRepo;
            //_billingRepo = billingRepo;
            //_reportRepo = reportRepo;
        }

        public async Task<IActionResult> Index()
        {
            // Get all pantry items
            var allItems = await _pantryRepo.GetAllAsync();

            // Low stock items (quantity < 5)
            var lowStockItems = allItems.Where(i => i.Quantity < 5).ToList();

            // Expiring soon items (expiry within 10 days)
            var expiringItems = allItems
                .Where(i => (i.ExpiryDate - DateTime.Now).TotalDays <= 10)
                .ToList();

            // Admin stats placeholders (replace with real repo calls)
            var totalOrders = 120;
            var totalRevenue = 25000m;
            var popularItemNames = new List<string> { "Tea", "Coffee", "Biscuit" };
            var popularItemCounts = new List<int> { 50, 40, 30 };

            // Build HomeViewModel
            var vm = new HomeViewModel
            {
                AllItems = allItems.ToList(),
                PantryItems = allItems.ToList(),
                LowStockItems = lowStockItems,
                ExpiringItems = expiringItems,

                TotalOrders = totalOrders,
                LowStockCount = lowStockItems.Count,
                TotalRevenue = totalRevenue,

                PopularItemNames = popularItemNames,
                PopularItemCounts = popularItemCounts
            };

            return View(vm);
        }
    }
}
