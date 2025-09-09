using System.ComponentModel.DataAnnotations;

namespace PantryManagementSystem.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
