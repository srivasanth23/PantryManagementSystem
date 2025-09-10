using System;
using System.ComponentModel.DataAnnotations;

namespace PantryManagementSystem.Models.DTO
{
    public class OrderCreateDTO
    {
        [Required]
        public Guid PantryItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}