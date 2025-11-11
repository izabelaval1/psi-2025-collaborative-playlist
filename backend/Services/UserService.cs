using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Interfaces;
using MyApi.Models;

namespace MyApi.Services
{
    public class UserService : IUserService
    {
        private readonly PlaylistAppContext _db;

        public UserService(PlaylistAppContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Select(u => new UserDto { Id = u.Id, Username = u.Username, Role = u.Role })
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (u == null) return null;
            return new UserDto { Id = u.Id, Username = u.Username, Role = u.Role };
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return (false, "Not found");
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ChangeRoleAsync(int id, UserRole newRole)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return (false, "Not found");

            u.Role = newRole;
            _db.Users.Update(u);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}