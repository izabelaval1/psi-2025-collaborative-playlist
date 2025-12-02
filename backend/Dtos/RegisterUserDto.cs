using System.ComponentModel.DataAnnotations;

public record RegisterUserDto
{
    [Required] public string Username { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = default!;
}
