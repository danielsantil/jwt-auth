using Microsoft.EntityFrameworkCore;
using TestAuth.Entities;

namespace TestAuth.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions options) : base(options) { }

        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<TokenLogin> TokenLogin { get; set; }
    }
}