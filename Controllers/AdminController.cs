using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DatingApp_API.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(UserManager<User> userManager) : ControllerBase
    {
        [HttpGet("users-with-roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var usersWithRoles = await userManager.Users
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    Username = x.UserName,
                    Roles = x.UserRoles.Select(x => x.Role.Name).ToList(),
                }
                ).ToListAsync();
            return Ok(usersWithRoles);
        }

        [HttpPost("edit-roles/{username}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> EditRoles(string username,[FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(',').ToArray();

            var user = await userManager.FindByNameAsync(username);

            if (user == null) return BadRequest("No user with that username found");

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Could not add roles");

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles.");

            return Ok(await userManager.GetRolesAsync(user));
        }

        [HttpGet("photos-to-moderate")]
        [Authorize(Policy = "ModeratePhotoRole")]
        public ActionResult GetPhotosFromModeration()
        {
            return Ok("Only moderators can see this.");
        }
    }
}
