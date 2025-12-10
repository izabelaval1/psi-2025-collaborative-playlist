using MyApi.Dtos;
using MyApi.Models;

namespace MyApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<(bool Success, string? Error)> ChangeRoleAsync(int id, UserRole newRole);

        Task<UserDto?> UpdateProfileImageAsync(int userId, IFormFile imageFile, string webRootPath);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string query);
    }
}