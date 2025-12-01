using MyApi.Models;

namespace MyApi.Repositories
{
    /// <summary>
    /// Repository abstraction for user data access.
    /// Service layer should use this instead of DbContext directly.
    /// </summary>
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);


        // NEW:
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);

        Task AddAsync(User user);
        Task DeleteAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> ExistsAsync(int id);
        
        Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<int> ids);

        Task<bool> UpdateProfileImageAsync(int userId, string imagePath);

    }
}
