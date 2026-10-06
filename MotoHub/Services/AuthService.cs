using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotoHub.DAL;
using MotoHub.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MotoHub.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> Register(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return null;

            var userRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "user");

            if (userRole == null)
                throw new Exception("Роль user не знайдена.");

            var user = new DAL.Entities.User
            {
                Email = dto.Email,
                Password = dto.Password,
                RoleId = userRole.Id
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user.Id, user.Email, userRole.Name);
        }

        public async Task<string?> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    u.Password == dto.Password);

            if (user == null)
                return null;

            return GenerateToken(
                user.Id,
                user.Email,
                user.Role.Name);
        }

        private string GenerateToken(
            int userId,
            string email,
            string role)
        {
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
                throw new Exception("JWT ключ не налаштований.");

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}