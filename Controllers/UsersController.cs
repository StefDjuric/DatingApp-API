using DatingApp_API.Data;
using DatingApp_API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class UsersController(DataContext dbContext) : ControllerBase
    {
        private readonly DataContext _dbcontext = dbContext;

        [HttpGet] // api/users
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _dbcontext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")] // api/users/id
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _dbcontext.Users.SingleOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
