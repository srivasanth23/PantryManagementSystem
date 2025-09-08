using System;

namespace PantryManagementSystem.Models.DTO
{
    public class PantryItemReadDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
