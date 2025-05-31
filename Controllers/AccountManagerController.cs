using DatingApp_API.Data;
using DatingApp_API.Entities;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountManagerController(DataContext dbContext, ITokenService tokenService) : ControllerBase
    {
        private readonly DataContext _dbcontext = dbContext;

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username, registerDTO.Email))
            {
                return BadRequest("User already exists with that username or email.");
            }
            return Ok();

            //using var hmac = new HMACSHA512();

            //var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));

            //var user = new User()
            //{
            //    Email = registerDTO.Email.ToLower(),
            //    Password = hashedPassword,
            //    Username = registerDTO.Username.ToLower(),
            //    PasswordSalt = hmac.Key
            //};

            //await _dbcontext.Users.AddAsync(user);
            //await _dbcontext.SaveChangesAsync();

            //return new UserDTO()
            //{
            //    Username = user.Username,
            //    Token = tokenService.CreateToken(user)
            //};

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _dbcontext.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.EmailOrUsername.ToLower() || u.Username == loginDTO.EmailOrUsername.ToLower());

            if (user == null) return Unauthorized("No user found with that email or username.");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i < hashedPassword.Length; i++)
            {
                if (user.Password[i] != hashedPassword[i]) return Unauthorized("Wrong Password.");
            }

            return new UserDTO()
            {
                Username = user.Username,
                Token = tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username, string email)
        {
            return await _dbcontext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == email.ToLower());
        }
    }
    
}
