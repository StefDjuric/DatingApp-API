using DatingApp_API.Entities;

namespace DatingApp_API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
