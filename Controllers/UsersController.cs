using AutoMapper;
using DatingApp_API.Entities;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DatingApp_API.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(IUserRepository userRepository) : ControllerBase
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
    }
}
