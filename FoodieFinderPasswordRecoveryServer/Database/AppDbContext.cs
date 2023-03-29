using FoodieFinderPasswordRecoveryServer.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodieFinderPasswordRecoveryServer.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<PasswordRecovery> PasswordRecovery { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
