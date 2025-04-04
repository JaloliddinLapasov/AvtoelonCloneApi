
using AvtoelonCloneApi.Models;

namespace AvtoelonCloneApi.Models
{
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; } // Enum sifatida
        public Category Category { get; set; } // Enum sifatida
        public Location Location { get; set; } // Enum sifatida
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}