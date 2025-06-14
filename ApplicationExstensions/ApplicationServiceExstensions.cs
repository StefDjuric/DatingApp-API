using DatingApp_API.Data;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.ApplicationExstensions
{
    public static class ApplicationServiceExstensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection service, IConfiguration config)
        {
            service.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();

            // Db connection
            service.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DbConnectionString"));
            });

            service.AddCors();
            service.AddScoped<ITokenService, TokenService>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IPhotoService, PhotoService>();
            service.AddScoped<ILikesRepository, LikesRepository>();
            service.AddScoped<IMessageRepository, MessageRepository>();
            service.AddScoped<LogUserActivity>();
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            service.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            return service;
        }
        
    }
   
}
