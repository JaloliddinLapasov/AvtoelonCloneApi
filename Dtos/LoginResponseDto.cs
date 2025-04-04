namespace AvtoelonCloneApi.Dtos
{
    // Login muvaffaqiyatli bo‘lganda qaytadigan javob modeli
    public class LoginResponseDto
    {
        public UserDto User { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
