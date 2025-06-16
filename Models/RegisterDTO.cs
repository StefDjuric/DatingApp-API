using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.Models
{
    public class RegisterDTO
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength =4)]
        public required string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public required string Password { get; set; } = string.Empty;

        [Required]
        public required string KnownAs { get; set; }
        [Required]
        public required string Gender { get; set; }
        [Required]
        public  required string City { get; set; }
        [Required]
        public  required string Country { get; set; }
        [Required]
        public  required string DateOfBirth{ get; set; }


    }
}
