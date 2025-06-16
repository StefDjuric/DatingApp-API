using AutoMapper;
using DatingApp_API.ApplicationExstensions;
using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp_API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController(IUserRepository userRepository, 
        IMessageRepository messageRepository, IMapper mapper) : ControllerBase
    {
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;

        [HttpPost("send")]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            if (currentUsername == createMessageDTO.RecipientUsername.ToLower()) 
                return BadRequest("You can not message yourself.");

            if (currentUsername == null || createMessageDTO.RecipientUsername == null)
                return BadRequest("No username found in the token.");

            var sender = await _userRepository.GetUserByUsernameAsync(currentUsername);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null || sender == null || recipient.UserName == null || sender.UserName == null)
                return BadRequest("Can not send message at this time.");

            var message = new Message
            {
                RecipientUsername = recipient.UserName,
                SenderUsername = sender.UserName,
                Sender = sender,
                Recipient = recipient,
                Content = createMessageDTO.Content,
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));
            else return BadRequest("Could not send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetUserMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (messageParams.Username == null) return BadRequest("No username found in token");

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages);

            return Ok(messages);
        }

        [HttpGet("get-thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread([FromRoute]string username)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            if (currentUsername == null) return BadRequest("No username found in token.");

            var messages = await _messageRepository.GetMessageThread(currentUsername, username);

            return Ok(messages);
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var message = await _messageRepository.GetMessage(id);

            if (message == null)
            {
                return BadRequest("Can not delete this message.");
            }
            
            if(message.SenderUsername != username && message.RecipientUsername != username)
            {
                return Forbid();
            }

            if(message.SenderUsername == username) message.SenderDeleted = true;
            else if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message is { SenderDeleted: true, RecipientDeleted: true })
                _messageRepository.DeleteMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Could not delete message");
        }

    }
}
