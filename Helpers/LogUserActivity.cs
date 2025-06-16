using DatingApp_API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DatingApp_API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var resultContext = await next();

            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var userId = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null) return;

            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            if(repo == null) throw new Exception("No repository found in log user activity.");

            var user = await repo.GetUserByIdAsync(Convert.ToInt32(userId));

            if (user == null) return;

            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();

            
        }
    }
}
