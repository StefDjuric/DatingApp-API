using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp_API.Entities;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {


        public async Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable();

            query = query.Where(user => user.UserName != userParams.CurrentUsername);

            if(userParams.Gender != null)
            {
                query = query.Where(user => user.Gender == userParams.Gender);
            }

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.CreatedAt),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider),
                 userParams.PageNumber, userParams.PageSize);
        }

        

        public async Task<MemberDTO?> GetMemberByIdAsync(int id)
        {
            return await context.Users.Where(x => x.Id == id)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<MemberDTO?> GetMemberByUsernameAsync(string username)
        {
            return await context.Users.Where(x => x.NormalizedUserName == username.ToUpper())
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(); 
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await context.Users.Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());

        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await context.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
             return await context.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            context.Entry(user).State = EntityState.Modified;
        }
    }
}
