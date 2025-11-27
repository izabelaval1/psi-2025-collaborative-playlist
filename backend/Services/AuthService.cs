using MyApi.Dtos;
using MyApi.Models;
using MyApi.Repositories;

namespace MyApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokens;

        public AuthService(IUserRepository users, ITokenService tokens)
        {
            _users = users;
            _tokens = tokens;
        }

        public async Task<(bool Success, string? Error, LoginResponseDto? Result)> RegisterAsync(RegisterUserDto dto)
        {
            var username = dto.Username?.Trim() ?? string.Empty;  //ncrash'ina ant null reiksmiu
            var password = dto.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required", null);

            if(password != dto.ConfirmPassword)
            {
                return (false, "Passwords do not match", null);
            }

            if (await _users.ExistsByUsernameAsync(username))
            {
                return (false, "Username already exists", null);
            }

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.Host  // veiks, kai enume pridėsi 'User' kol kas visi host
            };

            await _users.AddAsync(user);

            var token = _tokens.Generate(user);
            var result = new LoginResponseDto
            {
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.Username, Role = user.Role }
            };
            return (true, null, result);
        }

        public async Task<(bool Success, string? Error, LoginResponseDto? Result)> LoginAsync(LoginUserDto dto)
        {
            var username = dto.Username?.Trim() ?? string.Empty;
            var password = dto.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required", null);

            var user = await _users.GetByUsernameAsync(username);
            if (user == null) return (false, "Invalid credentials", null);

            var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!ok) return (false, "Invalid credentials", null);

            var token = _tokens.Generate(user);
            var result = new LoginResponseDto
            {
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.Username, Role = user.Role }
            };
            return (true, null, result);
        }
    }
}
