using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DatingApp_API.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker tracker) : Hub
    {
        public async override Task OnConnectedAsync()
        {
            if (Context.User == null) throw new Exception("Could not find the user");

            bool isOnline = await tracker.UserConnected(Context.User?.FindFirst(ClaimTypes.Name)?.Value!, Context.ConnectionId);
            if (isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User?.FindFirst(ClaimTypes.Name)?.Value);

            var currentUsers = await tracker.GetOnlineUsers();

            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User == null) throw new Exception("Could not find the user");

            bool isOffline = await tracker.UserDisconnected(Context.User?.FindFirst(ClaimTypes.Name)?.Value!, Context.ConnectionId);
            if(isOffline) await Clients.Others.SendAsync("UserHasDisconnected", Context.User.FindFirst(ClaimTypes.Name)?.Value);  

            await base.OnDisconnectedAsync(exception);

        }
    }
}
