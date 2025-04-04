using System.ComponentModel.DataAnnotations;

    namespace AvtoelonCloneApi.Dtos
    {
        public class RegisterDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Parol kamida 6 belgidan iborat bo'lishi kerak")]
            public string Password { get; set; } = string.Empty;
        }

        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public class UserDto
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
        }

        public class LoginResponseDto
        {
            public UserDto User { get; set; } = null!;
            public string Token { get; set; } = string.Empty;
        }
    }
