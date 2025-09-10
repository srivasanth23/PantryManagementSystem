namespace PantryManagementSystem.Models.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid PantryItemId { get; set; }
        public string PantryItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public decimal Price { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
