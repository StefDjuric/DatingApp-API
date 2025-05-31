using AutoMapper;
using CloudinaryDotNet.Actions;
using DatingApp_API.Entities;
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
    public class UsersController(IUserRepository userRepository, IMapper mapper,
        IPhotoService photoService) : ControllerBase
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

        [HttpPost("upload-photo")]
        public async Task<ActionResult<PhotoDTO>> UploadPhotoToCloudinary(IFormFile file)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null) return BadRequest("No username found in token.");

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return BadRequest("Not able to update the user.");

            var uploadResult = await photoService.AddPhotoAsync(file);

            if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

            var photo = new Photo
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync()) 
                return CreatedAtAction(nameof(GetUserByUsername), new {username = user.Username}, mapper.Map<PhotoDTO>(photo));

            return BadRequest("Could not add photo.");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null) return BadRequest("No username found in token");

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return BadRequest("Could not find the user.");

            var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain == true);

            if (currentMainPhoto != null) currentMainPhoto.IsMain = false;

            var newMainPhoto = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (newMainPhoto == null || newMainPhoto.IsMain) return BadRequest("Can not use this photo as main");

            newMainPhoto.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Could not set main photo.");
            
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(username == null) return BadRequest("No username found in token.");

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return BadRequest("Could not find the user");

            var photoIdx = user.Photos.FindIndex(p => p.Id == photoId);

            if (photoIdx < 0) return BadRequest("No photo found with that id.");

            if (user.Photos[photoIdx].PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(user.Photos[photoIdx].PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.RemoveAt(photoIdx);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Something went wrong while deleting photo.");
        }

    }
}
