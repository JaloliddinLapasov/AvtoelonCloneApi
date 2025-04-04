// using System.ComponentModel.DataAnnotations;

//     namespace AvtoelonCloneApi.Dtos
//     {
//         public class RegisterDto
//         {
//             [Required(ErrorMessage = "Email majburiy")]
//             [EmailAddress(ErrorMessage = "Yaroqsiz email format")]
//             public string Email { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Foydalanuvchi nomi majburiy")]
//             [MinLength(3, ErrorMessage = "Foydalanuvchi nomi kamida 3 belgi bo'lishi kerak")]
//             public string Username { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Parol majburiy")]
//             [DataType(DataType.Password)]
//             [MinLength(6, ErrorMessage = "Parol kamida 6 belgi bo'lishi kerak")]
//             public string Password { get; set; } = string.Empty;

//             [DataType(DataType.Password)]
//             [Compare("Password", ErrorMessage = "Parollar mos kelmadi")]
//             public string ConfirmPassword { get; set; } = string.Empty;
//         }

//         public class LoginDto
//         {
//             [Required(ErrorMessage = "Email majburiy")]
//             [EmailAddress(ErrorMessage = "Yaroqsiz email format")]
//             public string Email { get; set; } = string.Empty;

//             [Required(ErrorMessage = "Parol majburiy")]
//             [DataType(DataType.Password)]
//             public string Password { get; set; } = string.Empty;
//         }

//         // Foydalanuvchi ma'lumotlarini qaytarish uchun
//         public class UserDto
//         {
//             public string Id { get; set; } = string.Empty;
//             public string Email { get; set; } = string.Empty;
//             public string Username { get; set; } = string.Empty;
//             // public List<string> Roles { get; set; } = new List<string>(); // Rollarni ham qo'shish mumkin
//         }

//         // Login muvaffaqiyatli bo'lganda qaytariladigan javob
//         public class LoginResponseDto
//         {
//             public UserDto User { get; set; } = null!;
//             public string Token { get; set; } = string.Empty;
//             public DateTime Expiration { get; set; }
//         }
//     }
