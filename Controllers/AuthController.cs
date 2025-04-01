using AvtoelonCloneApi.Dtos;
using AvtoelonCloneApi.Models;
using AvtoelonCloneApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // FindByEmailAsync uchun

namespace AvtoelonCloneApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [AllowAnonymous] // Bu kontrollerdagi endpointlar uchun autentifikatsiya talab qilinmaydi
        public class AuthController : ControllerBase
        {   private readonly IConfiguration _config;
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly ITokenService _tokenService; // TokenService ni inject qilish

            public AuthController(IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
            {
                _config=config;
                _userManager = userManager;
                _signInManager = signInManager;
                _tokenService = tokenService;
                
            }

            [HttpPost("register")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Register(RegisterDto registerDto)
            {
                // Email yoki Username bandligini tekshirish
                if (await _userManager.Users.AnyAsync(u => u.UserName == registerDto.Username))
                {
                    return BadRequest(new { Message = "Bu foydalanuvchi nomi band." });
                }
                 if (await _userManager.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return BadRequest(new { Message = "Bu email manzili ro'yxatdan o'tgan." });
                }


                var user = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    // Xatoliklarni aniqroq qaytarish
                    return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
                }

                // Foydalanuvchiga standart rol berish (masalan, "Member")
                // await _userManager.AddToRoleAsync(user, "Member"); // Rollarni oldindan yaratish kerak

                return Ok(new { Message = "Ro'yxatdan o'tish muvaffaqiyatli!" });
            }

            [HttpPost("login")]
            [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return Unauthorized(new { Message = "Email yoki parol xato." });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false); // Lockout ni o'chirish

                if (!result.Succeeded)
                {
                    return Unauthorized(new { Message = "Email yoki parol xato." });
                }

                // Token yaratish
                var token = await _tokenService.CreateToken(user);
                var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"] ?? "60"));

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Username = user.UserName!
                };

                return Ok(new LoginResponseDto
                {
                    User = userDto,
                    Token = token,
                    Expiration = expiration
                });
            }
        }
    }
