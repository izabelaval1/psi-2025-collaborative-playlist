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

    }
}
