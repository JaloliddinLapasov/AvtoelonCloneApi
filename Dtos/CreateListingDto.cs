// using System.ComponentModel.DataAnnotations;

//     namespace AvtoelonCloneApi.Dtos
//     {
//         // Yangi e'lon yaratish uchun DTO
//         public class CreateListingDto
//         {
//             [Required(ErrorMessage = "Sarlavha kiritilishi shart")]
//             [MaxLength(200)]
//             public string Title { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Kategoriya tanlanishi shart")]
//             public string Category { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Narx kiritilishi shart")]
//             [Range(0.01, double.MaxValue, ErrorMessage = "Narx 0 dan katta bo'lishi kerak")]
//             public decimal Price { get; set; }

//             [Required]
//             [MaxLength(10)]
//             public string Currency { get; set; } = "USD";

//             [Required(ErrorMessage = "Tavsif kiritilishi shart")]
//             [MinLength(20, ErrorMessage = "Tavsif kamida 20 belgidan iborat bo'lishi kerak")]
//             public string Description { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Joylashuv tanlanishi shart")]
//             public string Location { get; set; } = string.Empty;

//             // Rasmlar alohida endpoint orqali yuklanishi mumkin
//             // public List<IFormFile>? Images { get; set; }
//             public List<string>? ImageUrls { get; set; } // Vaqtincha URL larni qabul qilish uchun

//             [Required(ErrorMessage = "Aloqa uchun ism kiritilishi shart")]
//             public string ContactName { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Aloqa uchun telefon raqam kiritilishi shart")]
//             [Phone(ErrorMessage = "Yaroqli telefon raqam kiriting")]
//             public string ContactPhone { get; set; } = string.Empty;
//         }
//     }
