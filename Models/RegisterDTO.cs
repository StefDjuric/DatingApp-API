using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.Models
{
    public class RegisterDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public required string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public required string Password { get; set; }  = string.Empty ;  
    }
}
