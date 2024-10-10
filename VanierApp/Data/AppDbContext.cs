using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VanierApp.Models;

namespace VanierApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}

