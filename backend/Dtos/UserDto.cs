using MyApi.Models;

namespace MyApi.Dtos
{
    public record UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public UserRole Role { get; set; }

        public string? ProfileImage { get; set; }
    }
}