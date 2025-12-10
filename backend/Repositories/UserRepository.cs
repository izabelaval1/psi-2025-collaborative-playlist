using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PlaylistAppContext _db;

        public UserRepository(PlaylistAppContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        // NEW:
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // NEW:
        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _db.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        }

        public async Task<bool> UpdateProfileImageAsync(int userId, string imagePath)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            user.ProfileImage = imagePath;
            await _db.SaveChangesAsync();
            return true;
        }
        
        public async Task<IEnumerable<User>> SearchByUsernameAsync(string query, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<User>();

            return await _db.Users
                .Where(u => u.Username.ToLower().Contains(query.ToLower()))
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
