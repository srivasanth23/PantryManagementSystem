using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PantryManagementSystem.Models.Domain
{
    public class Billing
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }   // FK to Identity User (not in domain here)

        [Required, MaxLength(20)]
        public string Month { get; set; }   // e.g. "Sep-2025"

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime GeneratedDate { get; set; }
    }
}