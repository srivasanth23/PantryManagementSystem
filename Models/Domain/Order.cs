using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.Enums;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; } // must be string
    public Guid PantryItemId { get; set; }
    public PantryItem PantryItem { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? IssuedDate { get; set; }
}
