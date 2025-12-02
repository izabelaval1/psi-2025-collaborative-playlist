using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Services;
using System.Security.Claims;
using MyApi.Models;
using MyApi.Utils;
using Microsoft.AspNetCore.Hosting;


namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userservice; 
        public UsersController(IUserService users) => _userservice = users;

        [Authorize (Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _userservice.GetAllAsync();
            return Ok(list);
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Naudojam ClaimTypes.NameIdentifier, nes taip generuojame token'ą
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            var user = await _userservice.GetByIdAsync(userId);
            if (user == null) return NotFound();
            
            return Ok(user);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var u = await _userservice.GetByIdAsync(id);
            if (u == null) return NotFound();
            return Ok(u);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, ChangeRoleDto dto)
        {
            var (success, error) = await _userservice.ChangeRoleAsync(id, dto.Role);
            if (!success) return BadRequest(error);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _userservice.DeleteAsync(id);
            if (!success) return BadRequest(error);
            return NoContent();
        }


        [Authorize]
        [HttpPut("{id}/profile-image")]
        public async Task<IActionResult> UpdateProfileImage(int id, [FromForm] IFormFile? imageFile, [FromServices] IWebHostEnvironment env)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized("Invalid token");

            if (currentUserId != id && !User.IsInRole("Admin") && !User.IsInRole("Host"))
                return Forbid();

            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var dto = await _userservice.UpdateProfileImageAsync(id, imageFile, env.WebRootPath);
                if (dto == null) return NotFound();
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // iš serviso ateina validacijos klaidos
            }

        }
    }

}
