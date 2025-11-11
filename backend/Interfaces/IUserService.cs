using MyApi.Dtos;
using MyApi.Models;

namespace MyApi.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<(bool Success, string? Error)> ChangeRoleAsync(int id, UserRole newRole);
    }
}