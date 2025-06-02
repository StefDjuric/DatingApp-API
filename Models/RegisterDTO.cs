using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.Models
{
    public class RegisterDTO
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength =4)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string? KnownAs { get; set; }
        [Required]
        public  string? Gender { get; set; }
        [Required]
        public  string? City { get; set; }
        [Required]
        public  string? Country { get; set; }
        [Required]
        public  string? DateOfBirth{ get; set; }


    }
}
