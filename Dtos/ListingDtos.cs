// using System.ComponentModel.DataAnnotations;

//     namespace AvtoelonCloneApi.Dtos
//     {
//         // E'lonlarni API orqali qaytarish uchun (umumiy ro'yxat)
//         public class ListingSummaryDto
//         {
//             public int Id { get; set; }
//             public string Title { get; set; } = string.Empty;
//             public decimal Price { get; set; }
//             public string Currency { get; set; } = string.Empty;
//             public string Location { get; set; } = string.Empty;
//             public string? FirstImageUrl { get; set; } // Faqat birinchi rasm
//             public DateTime CreatedAt { get; set; }
//         }

//         // E'lon detalini API orqali qaytarish uchun
//         public class ListingDetailDto
//         {
//             public int Id { get; set; }
//             public string Title { get; set; } = string.Empty;
//             public string Category { get; set; } = string.Empty;
//             public decimal Price { get; set; }
//             public string Currency { get; set; } = string.Empty;
//             public string Description { get; set; } = string.Empty;
//             public string Location { get; set; } = string.Empty;
//             public List<string> ImageUrls { get; set; } = new List<string>();
//             public string ContactName { get; set; } = string.Empty;
//             public string ContactPhone { get; set; } = string.Empty; // Balki buni faqat avtorizatsiyadan o'tganlarga ko'rsatish kerakdir
//             public DateTime CreatedAt { get; set; }
//             public DateTime UpdatedAt { get; set; }
//             public string UserId { get; set; } = string.Empty;
//             public string? UserName { get; set; } // E'lon egasining nomi
//         }

//         // Yangi e'lon yaratish uchun DTO
//         public class CreateListingsDto
//         {
//             [Required(ErrorMessage = "Sarlavha majburiy")]
//             [MaxLength(200)]
//             public string Title { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Kategoriya majburiy")]
//             [MaxLength(100)]
//             public string Category { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Narx majburiy")]
//             [Range(0.01, double.MaxValue, ErrorMessage = "Narx 0 dan katta bo'lishi kerak")]
//             public decimal Price { get; set; }

//             [Required(ErrorMessage = "Valyuta majburiy")]
//             [MaxLength(10)]
//             public string Currency { get; set; } = "USD";

//             [Required(ErrorMessage = "Tavsif majburiy")]
//             [MinLength(20, ErrorMessage = "Tavsif kamida 20 belgi bo'lishi kerak")]
//             public string Description { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Joylashuv majburiy")]
//             [MaxLength(150)]
//             public string Location { get; set; } = string.Empty;

//             // Rasmlar URL larini qabul qilish (yoki fayl yuklash uchun alohida endpoint)
//             public List<string>? ImageUrls { get; set; }

//             [Required(ErrorMessage = "Aloqa uchun ism majburiy")]
//             [MaxLength(100)]
//             public string ContactName { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Aloqa uchun telefon majburiy")]
//             [Phone(ErrorMessage = "Yaroqli telefon raqam kiriting")]
//             [MaxLength(50)]
//             public string ContactPhone { get; set; } = string.Empty;
//         }

//          // E'lonni tahrirlash uchun DTO (CreateListingDto ga o'xshash bo'lishi mumkin)
//         public class UpdateListingDto : CreateListingDto // Yoki alohida yaratish
//         {
//             // Tahrirlash uchun qo'shimcha yoki o'zgartirilgan maydonlar bo'lishi mumkin
//         }
//     }
