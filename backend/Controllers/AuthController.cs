using Microsoft.AspNetCore.Mvc;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService auth) => _authService = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var (success, error, result) = await _authService.RegisterAsync(dto);
            if (!success) return BadRequest(error);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var (success, error, result) = await _authService.LoginAsync(dto);
            if (!success) return Unauthorized(error);
            return Ok(result);
        }
    }
}