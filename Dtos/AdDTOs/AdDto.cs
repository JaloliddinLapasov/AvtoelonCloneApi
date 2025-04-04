using System.ComponentModel.DataAnnotations;
using AvtoelonCloneApi.Models;

namespace AvtoelonCloneApi.Dtos.AdDTOs
{
    public class AdDto
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        // [StringLength(100)]

        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public Currency Currency { get; set; } // Enum sifatida
        [Required]
        public Category Category { get; set; } // Enum sifatida
        [Required]
        public Location Location { get; set; } // Enum sifatida
        [Required]
        public string? ContactName { get; set; }
        [Required]
        public string? ContactPhone { get; set; }
        [Required]
        public string? ImagePath { get; set; }
    }
}