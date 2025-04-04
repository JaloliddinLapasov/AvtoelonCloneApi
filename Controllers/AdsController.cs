using AvtoelonCloneApi.Data;
using AvtoelonCloneApi.Dtos.AdDTOs;
using AvtoelonCloneApi.Mappers;
using AvtoelonCloneApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AvtoelonCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/ads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds()
        {
            var ads = await _context.Ads.ToListAsync();
            return Ok(ads);
        }

        // GET: api/ads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAd(int id)
        {
            var ad = await _context.Ads.FindAsync(id);

            if (ad == null)
            {
                return NotFound(new { message = "E‘lon topilmadi." });
            }

            return Ok(ad);
        }

        // POST: api/ads
        [HttpPost]
        public async Task<IActionResult> PostAd([FromBody] AdDto adDTO, IFormFile file)
        {
            // Validatsiya: Enum qiymatlarni tekshirish
            // if (!Enum.IsDefined(typeof(Currency), adDTO.Currency))
            // {
            //     return BadRequest(new { message = "Noto‘g‘ri valyuta tanlandi. Faqat USD, UZS yoki RUB tanlanishi mumkin." });
            // }

            // if (!Enum.IsDefined(typeof(Category), adDTO.Category))
            // {
            //     return BadRequest(new { message = "Noto‘g‘ri kategoriya tanlandi. Faqat YengilAvtomobillar, EhtiyotQismlar yoki Xizmatlar tanlanishi mumkin." });
            // }

            // if (!Enum.IsDefined(typeof(Location), adDTO.Location))
            // {
            //     return BadRequest(new { message = "Noto‘g‘ri joylashuv tanlandi. Faqat belgilangan joylashuvlardan biri tanlanishi mumkin." });
            // }

            // // Boshqa maydonlar uchun oddiy validatsiya
            // if (string.IsNullOrWhiteSpace(adDTO.Title) || adDTO.Title.Length < 5)
            // {
            //     return BadRequest(new { message = "Sarlavha kamida 5 belgidan iborat bo‘lishi kerak." });
            // }

            // if (string.IsNullOrWhiteSpace(adDTO.Description) || adDTO.Description.Length < 20)
            // {
            //     return BadRequest(new { message = "Tavsif kamida 20 belgidan iborat bo‘lishi kerak." });
            // }

            // if (adDTO.Price <= 0)
            // {
            //     return BadRequest(new { message = "Narx 0 dan katta bo‘lishi kerak." });
            // }

            // if (string.IsNullOrWhiteSpace(adDTO.ContactName))
            // {
            //     return BadRequest(new { message = "Aloqa uchun ism kiritilishi shart." });
            // }

            // if (string.IsNullOrWhiteSpace(adDTO.ContactPhone) || !System.Text.RegularExpressions.Regex.IsMatch(adDTO.ContactPhone, @"^[+]?[0-9]{1,3}?[-.\s]?([0-9]{1,3}){2}([0-9]{1,4})$"))
            // {
            //     return BadRequest(new { message = "Yaroqli telefon raqam kiriting (masalan, +998901234567)." });
            // }
            var adModel = adDTO.ToAdFromAdDTO();

            // Rasmni saqlash
            if (file != null)
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }

                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxFileSize)
                {
                    return BadRequest("File size exceeds the maximum limit of 5MB.");
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }


                // var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                // var filePath = Path.Combine(imagePath, uniqueFileName);

                // using (var fileStream = new FileStream(filePath, FileMode.Create))
                // {
                //     await file.CopyToAsync(fileStream);
                // }

                // Bazaga faqat rasmning yo‘li saqlanadi
                // ad   DTO.ImagePath = $"/uploads/{uniqueFileName}";
                adModel.ImagePath = $"/images/{fileName}";
            }



            await _context.Ads.AddAsync(adModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAd), new { id = adModel.Id }, adModel);
        }

        // PUT: api/ads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(int id, [FromForm] Ad ad, IFormFile? images)
        {
            if (id != ad.Id)
            {
                return BadRequest(new { message = "E‘lon ID si mos kelmadi." });
            }

            var existingAd = await _context.Ads.FindAsync(id);
            if (existingAd == null)
            {
                return NotFound(new { message = "E‘lon topilmadi." });
            }

            // Validatsiya: Enum qiymatlarni tekshirish
            if (!Enum.IsDefined(typeof(Currency), ad.Currency))
            {
                return BadRequest(new { message = "Noto‘g‘ri valyuta tanlandi. Faqat USD, UZS yoki RUB tanlanishi mumkin." });
            }

            if (!Enum.IsDefined(typeof(Category), ad.Category))
            {
                return BadRequest(new { message = "Noto‘g‘ri kategoriya tanlandi. Faqat YengilAvtomobillar, EhtiyotQismlar yoki Xizmatlar tanlanishi mumkin." });
            }

            if (!Enum.IsDefined(typeof(Location), ad.Location))
            {
                return BadRequest(new { message = "Noto‘g‘ri joylashuv tanlandi. Faqat belgilangan joylashuvlardan biri tanlanishi mumkin." });
            }

            // Boshqa maydonlar uchun validatsiya
            if (string.IsNullOrWhiteSpace(ad.Title) || ad.Title.Length < 5)
            {
                return BadRequest(new { message = "Sarlavha kamida 5 belgidan iborat bo‘lishi kerak." });
            }

            if (string.IsNullOrWhiteSpace(ad.Description) || ad.Description.Length < 20)
            {
                return BadRequest(new { message = "Tavsif kamida 20 belgidan iborat bo‘lishi kerak." });
            }

            if (ad.Price <= 0)
            {
                return BadRequest(new { message = "Narx 0 dan katta bo‘lishi kerak." });
            }

            if (string.IsNullOrWhiteSpace(ad.ContactName))
            {
                return BadRequest(new { message = "Aloqa uchun ism kiritilishi shart." });
            }

            if (string.IsNullOrWhiteSpace(ad.ContactPhone) || !System.Text.RegularExpressions.Regex.IsMatch(ad.ContactPhone, @"^[+]?[0-9]{1,3}?[-.\s]?([0-9]{1,3}){2}([0-9]{1,4})$"))
            {
                return BadRequest(new { message = "Yaroqli telefon raqam kiriting (masalan, +998901234567)." });
            }

            // Rasmni yangilash
            if (images != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + images.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await images.CopyToAsync(fileStream);
                }

                // Eski rasmni o‘chirish (agar mavjud bo‘lsa)
                if (!string.IsNullOrEmpty(existingAd.ImagePath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, existingAd.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existingAd.ImagePath = $"/uploads/{uniqueFileName}";
            }

            // E‘lonni yangilash
            existingAd.Title = ad.Title;
            existingAd.Description = ad.Description;
            existingAd.Price = ad.Price;
            existingAd.Currency = ad.Currency;
            existingAd.Category = ad.Category;
            existingAd.Location = ad.Location;
            existingAd.ContactName = ad.ContactName;
            existingAd.ContactPhone = ad.ContactPhone;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
                {
                    return NotFound(new { message = "E‘lon topilmadi." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound(new { message = "E‘lon topilmadi." });
            }

            // Rasmni o‘chirish (agar mavjud bo‘lsa)
            if (!string.IsNullOrEmpty(ad.ImagePath))
            {
                var filePath = Path.Combine(_environment.WebRootPath, ad.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdExists(int id)
        {
            return _context.Ads.Any(e => e.Id == id);
        }
    }
}