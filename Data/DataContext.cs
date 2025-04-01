// using AvtoelonCloneApi.Models;
//     using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//     using Microsoft.EntityFrameworkCore;

//     namespace AvtoelonCloneApi.Data
//     {
//         // IdentityDbContext dan foydalanish User, Role kabi jadvallarni avtomatik qo'shadi
//         public class DataContext : IdentityDbContext<AppUser>
//         {
//             public DataContext(DbContextOptions<DataContext> options) : base(options) { }

//             public DbSet<Listing> Listings { get; set; }

//             protected override void OnModelCreating(ModelBuilder builder)
//             {
//                 base.OnModelCreating(builder); // Identity uchun kerak

//                 // Listing va AppUser o'rtasidagi bog'liqlik
//                 builder.Entity<Listing>()
//                     .HasOne(l => l.User)
//                     .WithMany() // Bir foydalanuvchining ko'p e'lonlari bo'lishi mumkin
//                     .HasForeignKey(l => l.UserId)
//                     .IsRequired(false) // Agar foydalanuvchi o'chsa, e'lon qolishi mumkin (yoki OnDelete behavior ni o'zgartiring)
//                     .OnDelete(DeleteBehavior.SetNull); // Yoki Cascade, Restrict...

//                  // ImageUrls ni JSON sifatida saqlash (EF Core 7+ da soddalashgan)
//                  builder.Entity<Listing>()
//                     .Property(l => l.ImageUrls)
//                     .HasConversion(
//                         v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
//                         v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
//                     );

//                 // Boshqa konfiguratsiyalar...
//             }
//         }
//     }
