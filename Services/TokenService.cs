using AvtoelonCloneApi.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    namespace AvtoelonCloneApi.Services
    {
        public interface ITokenService
        {
            Task<string> CreateToken(AppUser user);
        }

        public class TokenService : ITokenService
        {
            private readonly IConfiguration _config;
            private readonly SymmetricSecurityKey _key;
            private readonly UserManager<AppUser> _userManager; // Rollarni olish uchun

            public TokenService(IConfiguration config, UserManager<AppUser> userManager)
            {
                _config = config;
                var jwtKey = _config["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("JWT Key konfiguratsiyada topilmadi yoki bo'sh.");
                }
                _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                _userManager = userManager;
            }

            public async Task<string> CreateToken(AppUser user)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id), // Sub o'rniga NameId ishlatish keng tarqalgan
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!), // Foydalanuvchi nomi
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unikal token identifikatori
                };

                // Foydalanuvchi rollarini claimlarga qo'shish
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


                var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); // Kuchliroq algoritm

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"] ?? "60")), // Yaroqlilik muddati
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
        }
    }
