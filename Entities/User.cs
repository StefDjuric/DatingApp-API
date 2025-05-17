namespace DatingApp_API.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public required byte[] Password { get; set; }

        public required byte[] PasswordSalt { get; set; }

        public required string Email { get; set; }
    }
}
