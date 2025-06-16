using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.Models
{
    public class LoginDTO
    {
        [Required]
        public required string EmailOrUsername { get; set; } = string.Empty;
        [Required]
        public required string Password { get; set; } = string.Empty;
    }
}
