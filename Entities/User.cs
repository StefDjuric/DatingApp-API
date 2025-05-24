﻿using DatingApp_API.ApplicationExstensions;

namespace DatingApp_API.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public byte[] Password { get; set; } = [];

        public byte[] PasswordSalt { get; set; } = [];

        public required string Email { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public required string KnownAs { get; set; }
        public DateTime CreatedAt {  get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public required string Gender {  get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; } 
        public string? LookingFor { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public List<Photo> Photos { get; set; } = [];

        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}
    }
}
