using System;

namespace PantryManagementSystem.Models.DTO
{
    public class RequestOrderDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }

        // Quantity user wants to request
        public int Quantity { get; set; }
    }
}
