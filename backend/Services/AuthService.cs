using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Interfaces;
using MyApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly PlaylistAppContext _db;
        private readonly IConfiguration _config;

        public AuthService(PlaylistAppContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<(bool Success, string? Error, LoginResponseDto? Result)> RegisterAsync(RegisterUserDto dto)
        {
            dto.Username = dto.Username.Trim();

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return (false, "Username already exists", null);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = GenerateJwt(user);

            var result = new LoginResponseDto
            {
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.Username, Role = user.Role }
            };

            return (true, null, result);
        }

        public async Task<(bool Success, string? Error, LoginResponseDto? Result)> LoginAsync(LoginUserDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return (false, "Invalid credentials", null);

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, "Invalid credentials", null);

            var token = GenerateJwt(user);
            var result = new LoginResponseDto
            {
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.Username, Role = user.Role }
            };

            return (true, null, result);
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}