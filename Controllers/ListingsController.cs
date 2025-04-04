// using AvtoelonCloneApi.Data;
//     using AvtoelonCloneApi.Dtos;
//     using AvtoelonCloneApi.Models;
//     using Microsoft.AspNetCore.Authorization;
//     using Microsoft.AspNetCore.Mvc;
//     using Microsoft.EntityFrameworkCore;
//     using System.Security.Claims;

//     namespace AvtoelonCloneApi.Controllers
//     {
//         [Route("api/[controller]")]
//         [ApiController]
//         public class ListingsController : ControllerBase
//         {
//             private readonly AppDataContext _context;

//             public ListingsController(AppDataContext context)
//             {
//                 _context = context;
//             }

//             // GET: api/listings
//             // GET: api/listings?category=yengil-avtomobillar
//             [HttpGet]
//             [AllowAnonymous] // Hamma ko'rishi mumkin
//             public async Task<ActionResult<IEnumerable<ListingDto>>> GetListings([FromQuery] string? category)
//             {
//                 var query = _context.Listings.AsQueryable();

//                 if (!string.IsNullOrEmpty(category))
//                 {
//                     query = query.Where(l => l.Category.ToLower() == category.ToLower());
//                 }

//                 var listings = await query
//                     .OrderByDescending(l => l.CreatedAt)
//                     .Select(l => new ListingDto // AutoMapper ishlatish yaxshiroq
//                     {
//                         Id = l.Id,
//                         Title = l.Title,
//                         Category = l.Category,
//                         Price = l.Price,
//                         Currency = l.Currency,
//                         Description = l.Description.Length > 100 ? l.Description.Substring(0, 100) + "..." : l.Description, // Qisqa tavsif
//                         Location = l.Location,
//                         ImageUrls = l.ImageUrls.Take(1).ToList(), // Faqat birinchi rasmni ko'rsatish
//                         CreatedAt = l.CreatedAt,
//                         UserId = l.UserId,
//                         UserName = l.User != null ? l.User.UserName : null
//                     })
//                     .ToListAsync();

//                 return Ok(listings);
//             }

//             // GET: api/listings/5
//             [HttpGet("{id}")]
//             [AllowAnonymous]
//             public async Task<ActionResult<ListingDto>> GetListing(int id)
//             {
//                 var listing = await _context.Listings
//                     .Include(l => l.User) // Foydalanuvchi ma'lumotini olish uchun
//                     .FirstOrDefaultAsync(l => l.Id == id);

//                 if (listing == null)
//                 {
//                     return NotFound();
//                 }

//                 // AutoMapper ishlatish tavsiya etiladi
//                 var listingDto = new ListingDto
//                 {
//                     Id = listing.Id,
//                     Title = listing.Title,
//                     Category = listing.Category,
//                     Price = listing.Price,
//                     Currency = listing.Currency,
//                     Description = listing.Description,
//                     Location = listing.Location,
//                     ImageUrls = listing.ImageUrls,
//                     ContactName = listing.ContactName, // Ehtiyot bo'ling, kim ko'rishi kerak?
//                     CreatedAt = listing.CreatedAt,
//                     UserId = listing.UserId,
//                     UserName = listing.User?.UserName
//                 };


//                 return Ok(listingDto);
//             }

//             // POST: api/listings
//             [HttpPost]
//             [Authorize] // Faqat tizimga kirgan foydalanuvchilar e'lon qo'shishi mumkin
//             public async Task<ActionResult<ListingDto>> PostListing(CreateListingDto createListingDto)
//             {
//                 var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // JWT dan foydalanuvchi ID sini olish

//                 if (userId == null)
//                 {
//                     return Unauthorized("Foydalanuvchi topilmadi.");
//                 }

//                 var listing = new Listing
//                 {
//                     Title = createListingDto.Title,
//                     Category = createListingDto.Category,
//                     Price = createListingDto.Price,
//                     Currency = createListingDto.Currency,
//                     Description = createListingDto.Description,
//                     Location = createListingDto.Location,
//                     ImageUrls = createListingDto.ImageUrls ?? new List<string>(), // Vaqtincha
//                     ContactName = createListingDto.ContactName,
//                     ContactPhone = createListingDto.ContactPhone,
//                     CreatedAt = DateTime.UtcNow,
//                     UserId = userId // E'lonni joriy foydalanuvchiga bog'lash
//                 };

//                 _context.Listings.Add(listing);
//                 await _context.SaveChangesAsync();

//                 // Yaratilgan e'lonni DTO sifatida qaytarish (AutoMapper bilan yaxshiroq)
//                  var listingDto = new ListingDto
//                 {
//                     Id = listing.Id,
//                     Title = listing.Title,
//                     Category = listing.Category,
//                     Price = listing.Price,
//                     Currency = listing.Currency,
//                     Description = listing.Description,
//                     Location = listing.Location,
//                     ImageUrls = listing.ImageUrls,
//                     ContactName = listing.ContactName,
//                     CreatedAt = listing.CreatedAt,
//                     UserId = listing.UserId,
//                     UserName = User.FindFirstValue(ClaimTypes.Name) // Yoki User dan topish
//                 };


//                 // CreatedAtAction yordamida 201 Created statusi va Location headerini qaytarish
//                 return CreatedAtAction(nameof(GetListing), new { id = listing.Id }, listingDto);
//             }

//             // PUT: api/listings/5 (Tahrirlash uchun) - Qo'shilmagan
//             // DELETE: api/listings/5 (O'chirish uchun) - Qo'shilmagan
//         }
//     }
