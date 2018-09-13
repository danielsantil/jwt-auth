using JwtAuthModels.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.DataContext
{
    public class JwtAuthDbContext : DbContext
    {
        public JwtAuthDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Token> Token { get; set; }
    }
}