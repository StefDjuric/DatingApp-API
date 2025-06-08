using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Core;
using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
    public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
    {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;
        public void AddLike(UserLike like)
        {
            _context.Likes.Add(like);
        }

        public void DeleteLike(UserLike like)
        {
            _context.Likes.Remove(like);
        }

        public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            return await _context.Likes
                .Where(l => l.SourceUserId == currentUserId)
                .Select(x => x.TargetUserId)
                .ToListAsync();
        }

        public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes
                .FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<MemberDTO>> GetUserLikes(LikeParams likeParams)
        {
            var likes = _context.Likes.AsQueryable();
            var users = _context.Users.AsQueryable();

            IQueryable<MemberDTO> query;

            if (likeParams.Predicate== "liked")
            {
                // Users that the current user has liked
                query = likes
                   .Where(like => like.SourceUserId == likeParams.UserId)
                   .Select(like => like.TargetUser)
                   .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider);
                    
            }

            else if (likeParams.Predicate == "likedBy")
            {
                // Users that have liked the current user
                query = likes
                    .Where(like => like.TargetUserId == likeParams.UserId)
                    .Select(like => like.SourceUser)
                    .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider);                   
            }

            else
            {
                // query approach for mutual likes
                query = (from like1 in likes
                              join like2 in likes on new { UserId1 = like1.SourceUserId, UserId2 = like1.TargetUserId }
                                                  equals new { UserId1 = like2.TargetUserId, UserId2 = like2.SourceUserId }
                              where like1.SourceUserId == likeParams.UserId
                              select like1.TargetUser)
                             .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider);
                             
            }

            return await  PagedList<MemberDTO>.CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
