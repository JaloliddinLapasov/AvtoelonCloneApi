using AvtoelonCloneApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvtoelonCloneApi.Data
{
    // IdentityDbContext dan meros olamiz, chunki foydalanuvchilarni Identity bilan boshqaryapmiz
    public class AuthDbContext : IdentityDbContext<AppUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        // Agar sizda boshqa modellaringiz bo‘lsa, shu yerga DbSet ko‘rinishida qo‘shasiz
        // Masalan:
        // public DbSet<Ad> Ads { get; set; } = null!;
    }
}
