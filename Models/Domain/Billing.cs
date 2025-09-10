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

        [NotMapped]   // not stored in DB, fetched from Identity
        public string UserEmail { get; set; }

        [Required, MaxLength(20)]
        public string Month { get; set; }   // e.g. "Sep-2025"

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; } = 10m;

        [Required]
        public DateTime GeneratedDate { get; set; }
    }
}