namespace MyApi.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error, LoginResponseDto? Result)> RegisterAsync(RegisterUserDto dto);
        Task<(bool Success, string? Error, LoginResponseDto? Result)> LoginAsync(LoginUserDto dto);
    }
}
