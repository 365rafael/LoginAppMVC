using LoginAppMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginAppMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
    }
}
