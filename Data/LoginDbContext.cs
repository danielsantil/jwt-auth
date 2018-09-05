using Microsoft.EntityFrameworkCore;
using TestAuth.Entities;

namespace TestAuth.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions options) : base(options) { }

        public DbSet<LoginModel> Login { get; set; }
    }
}