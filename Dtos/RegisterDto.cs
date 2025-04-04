using System.ComponentModel.DataAnnotations;

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
