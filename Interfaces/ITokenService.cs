using DatingApp_API.Entities;

namespace DatingApp_API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
