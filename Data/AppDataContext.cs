// using AvtoelonCloneApi.Models;
//     using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//     using Microsoft.EntityFrameworkCore;
// using System.Text.Json;

// namespace AvtoelonCloneApi.Data
//     {
//         // 1. IdentityDbContext<AppUser> dan to'g'ri meros olish
//         public class AppDataContext : IdentityDbContext<AppUser>
//         {
//          public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

//             // 3. DbSet null bo'lmasligi kerak (null forgiving operator !) yoki = default!;
//             public DbSet<Listing> Listings { get; set; } = default!;

//             // 4. OnModelCreating metodiga 'override' kalit so'zi qo'shilishi kerak
            

//         // CS1998 xatoligini oldini olish uchun SeedData metodi (agar mavjud bo'lsa)
//         // Agar bu metod sizda bo'lmasa, bu qismni e'tiborsiz qoldiring
//         // Agar async bo'lishi shart bo'lmasa:
//         /*
//         public static void SeedData(AppDataContext context)
//         {
//             // await ishlatilmaydigan sinxron kod
//         }
//         */
//         // Agar async bo'lishi kerak bo'lsa va await ishlatilsa:
//         /*
//         public static async Task SeedDataAsync(AppDataContext context)
//         {
//             if (!await context.Listings.AnyAsync()) // Misol uchun await
//             {
//                 // ... ma'lumot qo'shish ...
//                 await context.SaveChangesAsync();
//             }
//         }
//         */
//     }

//     public class IdentityDbContext<T>
//     {
//         internal void OnModelCreating(ModelBuilder builder)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }
