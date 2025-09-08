using PantryManagementSystem.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PantryManagementSystem.Models.Domain
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }   // FK to Identity User (not in domain here)

        [Required]
        public Guid PantryItemId { get; set; }  // FK to PantryItem

        [Required]
        public int Quantity { get; set; }

        [Required, MaxLength(20)]
        public OrderStatus Status { get; set; }   // Pending, Approved, Issued

        [Required]
        public DateTime RequestDate { get; set; }

        public DateTime? IssuedDate { get; set; }

        // Navigation properties
        public PantryItem PantryItem { get; set; }
    }
}
