using AvtoelonCloneApi.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    namespace AvtoelonCloneApi.Data
    {
        public class AppDataContext : IdentityDbContext<AppUser> // IdentityDbContext dan meros olish
        {
            public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
            {
            }

            // DbSet lar (jadvallar)
            public DbSet<Listing> Listings { get; set; } = default!; // Null forgiving operator

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder); // Identity konfiguratsiyasi uchun muhim

                // Listing konfiguratsiyasi
                builder.Entity<Listing>(entity =>
                {
                    entity.HasKey(e => e.Id); // Primary key

                    entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                    entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                    entity.Property(e => e.Currency).IsRequired().HasMaxLength(10).HasDefaultValue("USD");
                    entity.Property(e => e.Description).IsRequired();
                    entity.Property(e => e.Location).IsRequired().HasMaxLength(150);
                    entity.Property(e => e.ContactName).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.ContactPhone).IsRequired().HasMaxLength(50);
                    entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); // DB da vaqtni olish
                    entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                    // ImageUrlsJson ni DB ga saqlash (ImageUrls NotMapped)
                    entity.Property(e => e.ImageUrlsJson).HasColumnName("ImageUrls"); // DB dagi ustun nomi

                    // Listing va AppUser o'rtasidagi bog'liqlik (One-to-Many)
                    entity.HasOne(l => l.User)
                          .WithMany(u => u.Listings) // AppUser dagi Listings propertysi
                          .HasForeignKey(l => l.UserId)
                          .IsRequired() // Har bir e'lonning egasi bo'lishi shart
                          .OnDelete(DeleteBehavior.Cascade); // Foydalanuvchi o'chsa, uning e'lonlari ham o'chadi (yoki Restrict)

                    // Indexlar (qidiruvni tezlashtirish uchun)
                    entity.HasIndex(l => l.Category);
                    entity.HasIndex(l => l.Location);
                    entity.HasIndex(l => l.Price);
                    entity.HasIndex(l => l.CreatedAt);
                });

                // Boshqa Identity yoki model konfiguratsiyalari...
            }

             // Ma'lumotlar bazasiga o'zgarishlarni saqlashdan oldin UpdatedAt ni yangilash
            public override int SaveChanges()
            {
                UpdateTimestamps();
                return base.SaveChanges();
            }

            public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                UpdateTimestamps();
                return base.SaveChangesAsync(cancellationToken);
            }

            private void UpdateTimestamps()
            {
                var entries = ChangeTracker
                    .Entries()
                    .Where(e => e.Entity is Listing && (
                            e.State == EntityState.Added
                            || e.State == EntityState.Modified));

                foreach (var entityEntry in entries)
                {
                    ((Listing)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((Listing)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
