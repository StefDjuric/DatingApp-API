using DatingApp_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
