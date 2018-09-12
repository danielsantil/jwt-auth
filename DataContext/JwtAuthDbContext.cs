using Microsoft.EntityFrameworkCore;
using JwtAuth.Entities.Data;

namespace JwtAuth.DataContext
{
    public class JwtAuthDbContext : DbContext
    {
        public JwtAuthDbContext(DbContextOptions options) : base(options) { }

        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<TokenLogin> TokenLogin { get; set; }
    }
}