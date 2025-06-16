using AutoMapper;
using DatingApp_API.Data;
using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Controllers
{
    [ServiceFilter(typeof (LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountManagerController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username, registerDTO.Email))
            {
                return BadRequest("User already exists with that username or email.");
            }

            var user = mapper.Map<User>(registerDTO);

            user.UserName = registerDTO.Username.ToLower();
            user.NormalizedUserName = registerDTO.Username.ToUpper();
            user.NormalizedEmail = registerDTO.Email.ToUpper();
                    

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) return BadRequest("Coult not register user.");

            return new UserDTO()
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = registerDTO.KnownAs,
                Gender = user.Gender,
            };


        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.NormalizedEmail == loginDTO.EmailOrUsername.ToUpper() || u.NormalizedUserName == loginDTO.EmailOrUsername.ToUpper());

            if (user == null || user.UserName == null) return Unauthorized("No user found with that email or username.");

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result) return Unauthorized();

            return new UserDTO()
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        private async Task<bool> UserExists(string username, string email)
        {
            return await userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper() || u.NormalizedEmail == email.ToUpper());
        }
    }
    
}
