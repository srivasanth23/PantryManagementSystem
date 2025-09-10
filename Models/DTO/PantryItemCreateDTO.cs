using System;
using System.ComponentModel.DataAnnotations;

namespace PantryManagementSystem.Models.DTO
{
    public class PantryItemCreateDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}