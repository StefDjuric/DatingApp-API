using DatingApp_API.ApplicationExstensions;
using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController(ILikesRepository likesRepository) : ControllerBase
    {
        private readonly ILikesRepository _likesRepository = likesRepository;

        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (sourceUserId == targetUserId) return BadRequest("You can not like your own profile.");

            var existingLike = await _likesRepository.GetUserLike(sourceUserId, targetUserId);

            if (existingLike == null)
            {
                var like = new UserLike
                {
                    TargetUserId = targetUserId,
                    SourceUserId = sourceUserId,
                };

                _likesRepository.AddLike(like);
            }
            else
            {
                _likesRepository.DeleteLike(existingLike);
            }

            if (await _likesRepository.SaveChanges()) return Ok();

            else return BadRequest("Could not toggle like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return Ok(await _likesRepository.GetCurrentUserLikeIds(currentUserId));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {
            likeParams.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var userLikes = await _likesRepository.GetUserLikes(likeParams);

            Response.AddPaginationHeader(userLikes);

            return Ok(userLikes);
        }   

    }
}
