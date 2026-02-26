using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EduShareBackend.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AplicationDbContext _context;

        public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AplicationDbContext context)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task RegisterAsync(UserRegisterDTO registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new Exception("Korisnik već postoji.");

            var newUser = new Users
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                SecurityStamp = Guid.NewGuid().ToString(),
                DateRegistration = DateTime.UtcNow,
                DepartmentId = registerDto.DepartmentId
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var userRole = new UserRoles { UserId = newUser.UserId, RoleId = registerDto.RoleId };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenDTO> LoginAsync(UserLoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Neispravni podaci.");

            var refreshToken = CreateSecureRandomString();
            user.RefreshToken = refreshToken;
            user.TokenCreated = DateTime.UtcNow;
            user.TokenExpires = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            SetTokenCookie("refreshToken", refreshToken, 10080);
            return await PrepareAuthResponse(user);
        }

        public async Task<TokenDTO> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.TokenExpires < DateTime.UtcNow)
                throw new SecurityTokenException("Sesija istekla.");

            return await PrepareAuthResponse(user);
        }

        public void ClearCookies()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("accessToken");
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken");
        }

        private async Task<TokenDTO> PrepareAuthResponse(Users user)
        {
            var expiry = DateTime.UtcNow.AddMinutes(1);
            var token = CreateJwtToken(user, expiry);
            SetTokenCookie("accessToken", token, 1);

            return new TokenDTO { Expiry = expiry };
        }

        private string CreateJwtToken(Users user, DateTime expiry)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string CreateSecureRandomString()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private void SetTokenCookie(string name, string token, int minutes)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(name, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(minutes)
            });
        }
   public async Task<object> GetCurrentUserAsync()
{
    var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim)) return null;

    var id = int.Parse(userIdClaim);
    var user = await _context.Users
        .Include(u => u.Department)
        .FirstOrDefaultAsync(u => u.UserId == id);

    if (user == null) return null;

    var roleName = await _context.UserRoles
        .Where(ur => ur.UserId == id)
        .Include(ur => ur.Role)
        .Select(ur => ur.Role.RoleName)
        .FirstOrDefaultAsync();

    return new 
    {
        userId = user.UserId,
        firstName = user.FirstName,
        lastName = user.LastName,
        email = user.Email,
        role = roleName ?? "Student",
        departmentName = user.Department?.DepartmentName ?? "Nema smera"
    };
}
    }
}