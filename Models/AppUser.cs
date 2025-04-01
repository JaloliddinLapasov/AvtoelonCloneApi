using Microsoft.AspNetCore.Identity;

    namespace AvtoelonCloneApi.Models
    {
        // ASP.NET Core Identity foydalanuvchi modeli
        public class AppUser : IdentityUser
        {
            // Qo'shimcha maydonlar kerak bo'lsa:
            // public string? FullName { get; set; }
            // public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

            // Navigation Property (agar kerak bo'lsa)
            public virtual ICollection<Listing>? Listings { get; set; }
        }
    }
