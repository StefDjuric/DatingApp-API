﻿using Microsoft.AspNetCore.Identity;

namespace DatingApp_API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
