using PantryManagementSystem.Models.Domain;

namespace PantryManagementSystem.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<PantryItem> PantryItems { get; set; } = new();
        public List<PantryItem> LowStockItems { get; set; } = new();

        // Staff Dashboard enhancements
        public List<PantryItem> AllItems { get; set; } = new();        
        public List<PantryItem> ExpiringItems { get; set; } = new();   

        // Admin stats
        public int TotalOrders { get; set; }
        public int LowStockCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<string> PopularItemNames { get; set; } = new();
        public List<int> PopularItemCounts { get; set; } = new();
    }
}
