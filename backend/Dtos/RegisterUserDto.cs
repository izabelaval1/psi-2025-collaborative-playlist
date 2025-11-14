using MyApi.Dtos;
using System.ComponentModel.DataAnnotations;

public record RegisterUserDto
{
    [Required] public string Username { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;
}
