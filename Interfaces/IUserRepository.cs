using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Models;

namespace DatingApp_API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);
        Task<bool> SaveAllAsync();

        Task<IEnumerable<User>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<MemberDTO?> GetMemberByUsernameAsync(string username);
        Task<MemberDTO?> GetMemberByIdAsync(int id);
        Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userParams);

    }
}
