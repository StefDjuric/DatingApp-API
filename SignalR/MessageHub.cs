using AutoMapper;
using DatingApp_API.Data;
using DatingApp_API.Entities;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DatingApp_API.SignalR
{
    public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository,
        IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["user"];
            if (Context.User == null || string.IsNullOrEmpty(otherUser)) throw new Exception("Can not join group");

            var groupName = GetGroupName(Context?.User?.FindFirst(ClaimTypes.Name)?.Value!, otherUser!);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await messageRepository.GetMessageThread(Context?.User?.FindFirst(ClaimTypes.Name)?.Value!, otherUser!);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var currentUsername = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("Could not get user.");

            if (currentUsername == createMessageDTO.RecipientUsername.ToLower())
                throw new HubException("You can not message yourself.");

            if (currentUsername == null || createMessageDTO.RecipientUsername == null)
                throw new HubException("No username found in the token.");
            var sender = await userRepository.GetUserByUsernameAsync(currentUsername);
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null || sender == null || recipient.UserName == null || sender.UserName == null)
                throw new HubException("Can not send message at this time.");

            var message = new Message
            {
                RecipientUsername = recipient.UserName,
                SenderUsername = sender.UserName,
                Sender = sender,
                Recipient = recipient,
                Content = createMessageDTO.Content,
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(groupName);

            if(group != null && group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);

                if(connections != null && connections?.Count != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new {username = sender.UserName, knownAs = sender.KnownAs});
                }
            }

                messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
            }
        
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("Cannot get username");
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

            if(group == null)
            {
                group = new Group { Name = groupName };
                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            if (await messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromGroup()
        {
            var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (connection != null && group != null)
            {
                messageRepository.RemoveConnection(connection);
                if (await messageRepository.SaveAllAsync()) return group;
            }

            throw new HubException("Could not remove from group.");
        }

        private string GetGroupName(string caller, string other)
        {
            var compareResult = string.CompareOrdinal(caller, other) < 0;
            return compareResult ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        
    }
}
