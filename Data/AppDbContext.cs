using AvtoelonCloneApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AvtoelonCloneApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Ad> Ads { get; set; }
    }
}