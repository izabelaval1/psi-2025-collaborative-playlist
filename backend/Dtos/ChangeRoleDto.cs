using MyApi.Models;

namespace MyApi.Dtos
{
    public record ChangeRoleDto
    {
        public UserRole Role { get; set; }
    }
}