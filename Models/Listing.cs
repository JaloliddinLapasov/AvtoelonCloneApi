using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace AvtoelonCloneApi.Models
    {
        public class Listing
        {
            public int Id { get; set; }

            [Required]
            [MaxLength(200)]
            public string Title { get; set; } = string.Empty;

            [Required]
            public string Category { get; set; } = string.Empty; // Masalan: "yengil-avtomobillar", "ehtiyot-qismlar"

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal Price { get; set; }

            [Required]
            [MaxLength(10)]
            public string Currency { get; set; } = "USD"; // Masalan: "USD", "UZS"

            [Required]
            public string Description { get; set; } = string.Empty;

            [Required]
            public string Location { get; set; } = string.Empty;

            // Rasm URL larini saqlash uchun (oddiy variant)
            // Haqiqiy loyihada rasmlarni alohida saqlash va boshqarish kerak
            public List<string> ImageUrls { get; set; } = new List<string>();

            [Required]
            public string ContactName { get; set; } = string.Empty;

            [Required]
            public string ContactPhone { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            // Foydalanuvchi bilan bog'lash (kim e'lonni joylagani)
            [Required]
            public string? UserId { get; set; }
            public AppUser? User { get; set; }
        }
    }
