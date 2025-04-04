namespace AvtoelonCloneApi.Dtos
{
    // Foydalanuvchi ma'lumotlarini frontga qaytarish uchun DTO
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
