﻿using DatingApp_API.Entities;
using DatingApp_API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DatingApp_API.Services
{
    public class TokenService(IConfiguration config, UserManager<User> userManager) : ITokenService
    {
        public async Task<string>CreateToken(User user)
        {
            var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey is not found");
            if (tokenKey.Length < 64) throw new Exception("TokenKey length is less than 64");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            if (user.UserName == null) throw new Exception("No username found.");

            var claims = new List<Claim> 
            { 
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName)
            };

            var roles = await userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);  

        }
    }
}
