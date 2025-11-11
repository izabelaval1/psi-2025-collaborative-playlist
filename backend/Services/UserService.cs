using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Utils;
using MyApi.Repositories;

namespace MyApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly GenericConverter<User, UserDto> _converter;

        public UserService(IUserRepository UserRepository)
        {
            _userRepository = UserRepository;
           _converter = new GenericConverter<User, UserDto>();
        }


        // grazina visus users kaip dto
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var usersList = users.ToList(); // Konvertuojam IEnumerable į List
            return _converter.ConvertAll(usersList, u => new UserDto { Id = u.Id, Username = u.Username, Role = u.Role });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) return null;
            return _converter.ConvertOne(u, u => new UserDto { Id = u.Id, Username = u.Username, Role = u.Role });
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) return (false, "Not found");
            await _userRepository.DeleteAsync(u);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ChangeRoleAsync(int id, UserRole newRole)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) return (false, "Not found");

            u.Role = newRole;
            await _userRepository.UpdateAsync(u);
            return (true, null);
        }
    }
}