namespace DatingApp_API.Models
{
    public class LoginDTO
    {
        public required string EmailOrUsername { get; set; }

        public required string Password { get; set; }
    }
}
