
using DatingApp_API.ApplicationExstensions;
using DatingApp_API.Data;
using DatingApp_API.Entities;
using DatingApp_API.Middleware;
using DatingApp_API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace DatingApp_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(opt =>
            {
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.AllowCredentials();
                opt.WithOrigins("http://localhost:4200", "https://localhost:4200");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<PresenceHub>("hubs/presence");
            app.MapHub<MessageHub>("hubs/message");


            //// SEED DB
            //using var scope = app.Services.CreateScope();
            //var services = scope.ServiceProvider;
            //try
            //{
            //    var context = services.GetRequiredService<DataContext>();
            //    var userManager = services.GetRequiredService<UserManager<User>>();
            //    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            //    await context.Database.MigrateAsync();
            //    await Seed.SeedUsers(userManager, roleManager);
            //}
            //catch (Exception ex)
            //{
            //    var logger = services.GetRequiredService<ILogger<Program>>();
            //    logger.LogError(ex, "An error occured during migration");
            //}

            app.Run();
        }
    }
}
