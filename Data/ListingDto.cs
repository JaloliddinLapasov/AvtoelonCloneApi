namespace AvtoelonCloneApi.Dtos
    {
        // E'lonlarni API orqali qaytarish uchun DTO
        public class ListingDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public List<string> ImageUrls { get; set; } = new List<string>();
            public string ContactName { get; set; } = string.Empty; // Balki buni hamma uchun ko'rsatmaslik kerakdir
            public DateTime CreatedAt { get; set; }
            public string? UserId { get; set; } // Kim joylaganini bilish uchun
            public string? UserName { get; set; } // Foydalanuvchi nomini ham qo'shish mumkin
        }
    }
