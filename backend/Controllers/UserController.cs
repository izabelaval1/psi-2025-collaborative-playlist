using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Services;
using System.Security.Claims;
using MyApi.Models;
using MyApi.Utils;

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
        public async Task<IActionResult> UpdateProfileImage(int id, [FromForm] IFormFile? imageFile)
        {
            // Pasiimam ID taip pat, kaip GetCurrentUser
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized("Invalid token");
            }
            

            // Only allow users to update their own profile image, unless they're an admin/host (sitas dar nesutvarkyta, visi user defaultu yra host)
            if (currentUserId != id)
            {
            // laikina
                var isAdminOrHost = User.IsInRole("Admin") || User.IsInRole("Host");
                if (!isAdminOrHost)
                {
                    return Forbid();
                }
            }
            
            var user = await _userservice.GetByIdAsync(id);
            if (user == null) return NotFound();

            if (imageFile != null)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file type. Only JPG, PNG, and GIF are allowed.");
                }
                
                // Validate file size (e.g., 5MB max)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("File size cannot exceed 5MB.");
                }

                // Ištrink seną nuotrauką jei yra
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    var oldPath = Path.Combine("wwwroot", user.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // Išsaugok naują
                var uploadsFolder = Path.Combine("wwwroot", "profiles");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                
                var imagePath = $"/profiles/{uniqueFileName}";

                var success = await _userservice.UpdateProfileImageAsync(id, imagePath);
                if (!success) return BadRequest("Failed to update profile image.");

                user.ProfileImage = imagePath;
            }
            
            var dto = new UserDto {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ProfileImage = user.ProfileImage
            };

            return Ok(dto);

        }
    }

}
