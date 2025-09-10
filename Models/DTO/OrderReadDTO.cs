namespace PantryManagementSystem.Models.DTO
{
    public class OrderReadDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }   // keep as Guid
        public string UserEmail { get; set; } // get email instead of username
        public string PantryItemName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? IssuedDate { get; set; }
    }
}
