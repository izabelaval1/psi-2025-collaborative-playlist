using MyApi.Dtos;

public record LoginUserDto
{
    public string? Username { get; set; } = null!;
    public string? Password { get; set; } = null!;
}
