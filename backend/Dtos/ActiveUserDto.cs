using MyApi.Models;

namespace MyApi.Dtos
{
    public record ActiveUserDto
    {
        public int UserId { get; init; }
        public string Username { get; init; } = null!;
        public UserRole Role { get; init; } = UserRole.Guest;
        public DateTime LastActive { get; init; }
    }
}
