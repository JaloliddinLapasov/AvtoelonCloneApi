// using System.ComponentModel.DataAnnotations;
// using AvtoelonCloneApi.Models;

// public class AdCreateDto
// {
//     [Required]
//     public string? Title { get; set; }

//     [Required]
//     public Category? Category { get; set; }

//     [Required]
//     [Range(0, double.MaxValue, ErrorMessage = "Narx to'g'ri kiritilishi kerak.")]
//     public decimal Price { get; set; }

//     [Required]
//     public Currency? Currency { get; set; }

//     [Required]
//     [MinLength(20, ErrorMessage = "Tavsif kamida 20 belgidan iborat bo'lishi kerak.")]
//     public string? Description { get; set; }

//     [Required]
//     public Location? Location { get; set; }

//     [Required]
//     [Phone]
//     public string? ContactPhone { get; set; }

//     [Required]
//     public string? ContactName { get; set; }

//     public List<string>? Images { get; set; }
//     public int Id { get; internal set; }
// }
