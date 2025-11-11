using MyApi.Models;

namespace MyApi.Interfaces
{
    /// <summary>
    /// Repository abstraction for user data access.
    /// Service layer should use this instead of DbContext directly.
    /// </summary>
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task DeleteAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> ExistsAsync(int id);
    }
}
