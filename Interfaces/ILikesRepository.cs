using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Models;

namespace DatingApp_API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int sourceUserId, int TargetUserId);
        Task<PagedList<MemberDTO>> GetUserLikes(LikeParams likeParams);
        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
        void AddLike(UserLike like);
        void DeleteLike(UserLike like);
        Task<bool> SaveChanges();
    }
}
