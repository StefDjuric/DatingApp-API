using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp_API.Entities;
using DatingApp_API.Interfaces;
using DatingApp_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {
        private readonly DataContext _context = context;

        public async Task<IEnumerable<MemberDTO>> GetAllMembersAsync()
        {
            return await _context.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<MemberDTO?> GetMemberByIdAsync(int id)
        {
            return await _context.Users.Where(x => x.Id == id)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<MemberDTO?> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users.Where(x => x.Username == username)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(); 
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(u => u.Photos).SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
             return await _context.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
