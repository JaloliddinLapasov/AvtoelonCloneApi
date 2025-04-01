using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace AvtoelonCloneApi.Models
    {
        public class Listing
        {
            [Key] // Primary Key
            public int Id { get; set; }

            [Required(ErrorMessage = "Sarlavha majburiy")]
            [MaxLength(200)]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Kategoriya majburiy")]
            [MaxLength(100)]
            public string Category { get; set; } = string.Empty; // Masalan: "yengil-avtomobillar"

            [Required(ErrorMessage = "Narx majburiy")]
            [Column(TypeName = "decimal(18,2)")] // Ma'lumotlar bazasida aniq tur
            [Range(0.01, double.MaxValue, ErrorMessage = "Narx 0 dan katta bo'lishi kerak")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Valyuta majburiy")]
            [MaxLength(10)]
            public string Currency { get; set; } = "USD"; // Default "USD"

            [Required(ErrorMessage = "Tavsif majburiy")]
            [MinLength(20, ErrorMessage = "Tavsif kamida 20 belgi bo'lishi kerak")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Joylashuv majburiy")]
            [MaxLength(150)]
            public string Location { get; set; } = string.Empty;

            // Rasmlar uchun (oddiy variant - URL lar ro'yxati JSON sifatida)
            public string? ImageUrlsJson { get; set; } // JSON string saqlash uchun

            [NotMapped] // Bu maydon DB ga yozilmaydi
            public List<string> ImageUrls
            {
                get => string.IsNullOrEmpty(ImageUrlsJson)
                       ? new List<string>()
                       : System.Text.Json.JsonSerializer.Deserialize<List<string>>(ImageUrlsJson) ?? new List<string>();
                set => ImageUrlsJson = System.Text.Json.JsonSerializer.Serialize(value);
            }


            [Required(ErrorMessage = "Aloqa uchun ism majburiy")]
            [MaxLength(100)]
            public string ContactName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Aloqa uchun telefon majburiy")]
            [Phone(ErrorMessage = "Yaroqli telefon raqam kiriting")]
            [MaxLength(50)]
            public string ContactPhone { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Foreign Key - Foydalanuvchi bilan bog'lanish
            [Required]
            public string UserId { get; set; } = string.Empty;

            // Navigation Property
            [ForeignKey("UserId")]
            public virtual AppUser? User { get; set; }
        }
    }
