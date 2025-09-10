namespace PantryManagementSystem.Models.DTO
{
    public class MyOrderDTO
    {
        public Guid Id { get; set; }
        public string PantryItemName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }   // store as string for easy display
        public DateTime RequestDate { get; set; }
        public DateTime? IssuedDate { get; set; }
    }
}
