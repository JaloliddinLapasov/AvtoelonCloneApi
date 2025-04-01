using AvtoelonCloneApi.Data;
    using AvtoelonCloneApi.Dtos;
    using AvtoelonCloneApi.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;

    namespace AvtoelonCloneApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ListingsController : ControllerBase
        {
            private readonly AppDataContext _context;
            private readonly ILogger<ListingsController> _logger; // Loglash uchun

            public ListingsController(AppDataContext context, ILogger<ListingsController> logger)
            {
                _context = context;
                _logger = logger;
            }

            // GET: api/listings?category=yengil-avtomobillar&location=Toshkent&page=1&pageSize=10
            [HttpGet]
            [AllowAnonymous] // Hamma ko'rishi mumkin
            [ProducesResponseType(typeof(IEnumerable<ListingSummaryDto>), StatusCodes.Status200OK)]
            public async Task<ActionResult<IEnumerable<ListingSummaryDto>>> GetListings(
                [FromQuery] string? category,
                [FromQuery] string? location,
                [FromQuery] decimal? minPrice,
                [FromQuery] decimal? maxPrice,
                [FromQuery] string? searchTerm,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
            {
                _logger.LogInformation("GetListings chaqirildi. Kategoriya: {Category}, Joylashuv: {Location}", category, location);

                var query = _context.Listings.AsNoTracking().AsQueryable(); // O'qish uchun optimallashtirish

                // Filtrlash
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(l => l.Category.ToLower() == category.ToLower());
                }
                if (!string.IsNullOrEmpty(location))
                {
                    query = query.Where(l => l.Location.ToLower() == location.ToLower());
                }
                 if (minPrice.HasValue)
                {
                    query = query.Where(l => l.Price >= minPrice.Value);
                }
                 if (maxPrice.HasValue)
                {
                    query = query.Where(l => l.Price <= maxPrice.Value);
                }
                 if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(l => l.Title.Contains(searchTerm) || l.Description.Contains(searchTerm));
                }


                // Sahifalash
                var totalItems = await query.CountAsync();
                var listings = await query
                    .OrderByDescending(l => l.CreatedAt) // Eng yangilari birinchi
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new ListingSummaryDto // AutoMapper ishlatish tavsiya etiladi
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Price = l.Price,
                        Currency = l.Currency,
                        Location = l.Location,
                        FirstImageUrl = l.ImageUrls.FirstOrDefault(), // Faqat birinchi rasm
                        CreatedAt = l.CreatedAt
                    })
                    .ToListAsync();

                 // Sahifalash ma'lumotlarini headerga qo'shish (ixtiyoriy)
                 Response.Headers.Append("X-Pagination-TotalItems", totalItems.ToString());
                 Response.Headers.Append("X-Pagination-PageSize", pageSize.ToString());
                 Response.Headers.Append("X-Pagination-CurrentPage", page.ToString());
                 Response.Headers.Append("X-Pagination-TotalPages", ((int)Math.Ceiling((double)totalItems / pageSize)).ToString());


                return Ok(listings);
            }

            // GET: api/listings/5
            [HttpGet("{id}")]
            [AllowAnonymous]
            [ProducesResponseType(typeof(ListingDetailDto), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<ListingDetailDto>> GetListing(int id)
            {
                 _logger.LogInformation("GetListing chaqirildi. ID: {Id}", id);

                var listing = await _context.Listings
                    .AsNoTracking()
                    .Include(l => l.User) // Foydalanuvchi ma'lumotini olish
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (listing == null)
                {
                     _logger.LogWarning("E'lon topilmadi. ID: {Id}", id);
                    return NotFound(new { Message = "E'lon topilmadi" });
                }

                // AutoMapper ishlatish tavsiya etiladi
                var listingDto = new ListingDetailDto
                {
                    Id = listing.Id,
                    Title = listing.Title,
                    Category = listing.Category,
                    Price = listing.Price,
                    Currency = listing.Currency,
                    Description = listing.Description,
                    Location = listing.Location,
                    ImageUrls = listing.ImageUrls,
                    ContactName = listing.ContactName,
                    ContactPhone = listing.ContactPhone, // Ehtiyot bo'ling!
                    CreatedAt = listing.CreatedAt,
                    UpdatedAt = listing.UpdatedAt,
                    UserId = listing.UserId,
                    UserName = listing.User?.UserName // Null check
                };

                return Ok(listingDto);
            }

            // POST: api/listings
            [HttpPost]
            [Authorize] // Faqat tizimga kirganlar e'lon qo'sha oladi
            [ProducesResponseType(typeof(ListingDetailDto), StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            public async Task<ActionResult<ListingDetailDto>> CreateListing(CreateListingDto createListingDto)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // JWT dan foydalanuvchi ID sini olish

                if (userId == null)
                {
                    // Bu holat [Authorize] tufayli kamdan-kam yuz beradi, lekin tekshirish yaxshi
                    return Unauthorized(new { Message = "Avtorizatsiyadan o'tilmagan" });
                }

                 _logger.LogInformation("CreateListing chaqirildi. Foydalanuvchi ID: {UserId}", userId);

                var listing = new Listing
                {
                    Title = createListingDto.Title,
                    Category = createListingDto.Category,
                    Price = createListingDto.Price,
                    Currency = createListingDto.Currency,
                    Description = createListingDto.Description,
                    Location = createListingDto.Location,
                    ImageUrls = createListingDto.ImageUrls ?? new List<string>(), // Null check
                    ContactName = createListingDto.ContactName,
                    ContactPhone = createListingDto.ContactPhone,
                    UserId = userId // E'lonni joriy foydalanuvchiga bog'lash
                    // CreatedAt va UpdatedAt avtomatik qo'yiladi (AppDataContext da)
                };

                _context.Listings.Add(listing);

                try
                {
                    await _context.SaveChangesAsync();
                     _logger.LogInformation("Yangi e'lon muvaffaqiyatli yaratildi. ID: {ListingId}", listing.Id);
                }
                catch (DbUpdateException ex)
                {
                     _logger.LogError(ex, "E'lonni saqlashda xatolik.");
                    return BadRequest(new { Message = "E'lonni saqlashda xatolik yuz berdi." });
                }


                // Yaratilgan e'lonni DTO sifatida qaytarish (AutoMapper bilan yaxshiroq)
                 var listingDto = new ListingDetailDto
                 {
                    Id = listing.Id,
                    Title = listing.Title,
                    Category = listing.Category,
                    Price = listing.Price,
                    Currency = listing.Currency,
                    Description = listing.Description,
                    Location = listing.Location,
                    ImageUrls = listing.ImageUrls,
                    ContactName = listing.ContactName,
                    ContactPhone = listing.ContactPhone,
                    CreatedAt = listing.CreatedAt,
                    UpdatedAt = listing.UpdatedAt,
                    UserId = listing.UserId,
                    UserName = User.FindFirstValue(ClaimTypes.Name) // Yoki User dan topish
                 };

                // 201 Created statusi va Location headerini qaytarish
                return CreatedAtAction(nameof(GetListing), new { id = listing.Id }, listingDto);
            }

            // PUT: api/listings/5
            [HttpPut("{id}")]
            [Authorize] // Faqat tizimga kirganlar tahrirlay oladi
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> UpdateListing(int id, UpdateListingDto updateListingDto)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                 _logger.LogInformation("UpdateListing chaqirildi. ID: {Id}, Foydalanuvchi ID: {UserId}", id, userId);


                var listingToUpdate = await _context.Listings.FindAsync(id);

                if (listingToUpdate == null)
                {
                     _logger.LogWarning("Tahrirlash uchun e'lon topilmadi. ID: {Id}", id);
                    return NotFound(new { Message = "E'lon topilmadi" });
                }

                // Foydalanuvchi o'zining e'lonini tahrirlayotganini tekshirish
                if (listingToUpdate.UserId != userId)
                {
                     _logger.LogWarning("Foydalanuvchi boshqa birovning e'lonini tahrirlashga urindi. E'lon ID: {Id}, Foydalanuvchi ID: {UserId}", id, userId);
                    return Forbid(); // 403 Forbidden
                }

                // Maydonlarni yangilash (AutoMapper bu yerda juda foydali bo'lardi)
                listingToUpdate.Title = updateListingDto.Title;
                listingToUpdate.Category = updateListingDto.Category;
                listingToUpdate.Price = updateListingDto.Price;
                listingToUpdate.Currency = updateListingDto.Currency;
                listingToUpdate.Description = updateListingDto.Description;
                listingToUpdate.Location = updateListingDto.Location;
                listingToUpdate.ImageUrls = updateListingDto.ImageUrls ?? listingToUpdate.ImageUrls; // Agar yangi URL lar kelmasa, eskisi qoladi
                listingToUpdate.ContactName = updateListingDto.ContactName;
                listingToUpdate.ContactPhone = updateListingDto.ContactPhone;
                // UpdatedAt avtomatik yangilanadi

                _context.Entry(listingToUpdate).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                     _logger.LogInformation("E'lon muvaffaqiyatli yangilandi. ID: {Id}", id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Parallel o'zgarishlar bo'lsa
                     _logger.LogError(ex, "E'lonni yangilashda concurrency xatoligi. ID: {Id}", id);
                    if (!await ListingExists(id))
                    {
                        return NotFound(new { Message = "E'lon topilmadi" });
                    }
                    else
                    {
                        // Boshqa turdagi concurrency xatoligi
                         return Conflict(new { Message = "E'lonni yangilashda xatolik (conflict)." });
                    }
                }
                 catch (DbUpdateException ex)
                {
                     _logger.LogError(ex, "E'lonni yangilashda xatolik. ID: {Id}", id);
                    return BadRequest(new { Message = "E'lonni yangilashda xatolik yuz berdi." });
                }

                return NoContent(); // 204 No Content - Muvaffaqiyatli yangilandi
            }


            // DELETE: api/listings/5
            [HttpDelete("{id}")]
            [Authorize] // Faqat tizimga kirganlar o'chira oladi
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> DeleteListing(int id)
            {
                 var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                 _logger.LogInformation("DeleteListing chaqirildi. ID: {Id}, Foydalanuvchi ID: {UserId}", id, userId);

                var listingToDelete = await _context.Listings.FindAsync(id);

                if (listingToDelete == null)
                {
                     _logger.LogWarning("O'chirish uchun e'lon topilmadi. ID: {Id}", id);
                    return NotFound(new { Message = "E'lon topilmadi" });
                }

                 // Foydalanuvchi o'zining e'lonini o'chirayotganini tekshirish
                if (listingToDelete.UserId != userId)
                {
                     _logger.LogWarning("Foydalanuvchi boshqa birovning e'lonini o'chirishga urindi. E'lon ID: {Id}, Foydalanuvchi ID: {UserId}", id, userId);
                    return Forbid(); // 403 Forbidden
                }

                _context.Listings.Remove(listingToDelete);

                 try
                {
                    await _context.SaveChangesAsync();
                     _logger.LogInformation("E'lon muvaffaqiyatli o'chirildi. ID: {Id}", id);
                }
                 catch (DbUpdateException ex)
                {
                     _logger.LogError(ex, "E'lonni o'chirishda xatolik. ID: {Id}", id);
                    return BadRequest(new { Message = "E'lonni o'chirishda xatolik yuz berdi." });
                }


                return NoContent(); // 204 No Content - Muvaffaqiyatli o'chirildi
            }

            // Yordamchi metod
            private async Task<bool> ListingExists(int id)
            {
                return await _context.Listings.AnyAsync(e => e.Id == id);
            }
        }
    }
