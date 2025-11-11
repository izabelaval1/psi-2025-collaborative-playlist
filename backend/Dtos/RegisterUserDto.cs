using MyApi.Dtos;

public record RegisterUserDto
{
    public string? Username { get; set; } = null!;
    public string? Password { get; set; } = null!;

}