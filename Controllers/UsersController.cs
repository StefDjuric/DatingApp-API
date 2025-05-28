using AutoMapper;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace DatingApp_API.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(IUserRepository userRepository, IMapper mapper) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        //private readonly IMapper _mapper = mapper;

        [HttpGet] // api/users
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await userRepository.GetAllMembersAsync();
            return Ok(users);
        }

        
        [HttpGet("{id:int}")] // api/users/id
        public async Task<ActionResult<MemberDTO>> GetUserById(int id)
        {
            var user = await _userRepository.GetMemberByIdAsync(id);

            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUserByUsername(string username)
        {
            var user = await _userRepository.GetMemberByUsernameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("edit-member")]
        public async Task<ActionResult> UpdateUser(MemberEditDTO memberEditDTO)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(username == null) return BadRequest("No username found in token.");

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if(user == null) return BadRequest("Could not find user");

            mapper.Map(memberEditDTO, user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update the user.");
        }
    }
}
