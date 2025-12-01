using MyApi.Dtos;
using MyApi.Models;
using MyApi.Utils;
using MyApi.Repositories;
using MyApi.Exceptions; // aptrinti usings
using Microsoft.AspNetCore.Http;
using System.IO;

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
            return _converter.ConvertAll(usersList, u => new UserDto { Id = u.Id, Username = u.Username, Role = u.Role, ProfileImage = u.ProfileImage });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) 
            {
                return null;
            }
            return _converter.ConvertOne(u, u => new UserDto { Id = u.Id, Username = u.Username, Role = u.Role, ProfileImage = u.ProfileImage });
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) 
            {
                return (false, "User not found");
            }
            await _userRepository.DeleteAsync(u);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ChangeRoleAsync(int id, UserRole newRole)
        {
            var u = await _userRepository.GetByIdAsync(id);
            if (u == null) 
            {
                return (false, "User not found");
            }

            u.Role = newRole;
            await _userRepository.UpdateAsync(u);
            return (true, null);
        }

        public async Task<UserDto?> UpdateProfileImageAsync(int id, IFormFile imageFile, string webRootPath)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // 1. Validacija
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only JPG, PNG, and GIF are allowed.");

            if (imageFile.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size cannot exceed 5MB.");

            // 2. Senos nuotraukos ištrynimas
            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                var oldPath = Path.Combine(webRootPath, user.ProfileImage.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            // 3. Naujo failo išsaugojimas
            var uploadsFolder = Path.Combine(webRootPath, "profiles");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var imagePath = $"/profiles/{uniqueFileName}";

            // 4. Atnaujinam DB per repo
            var success = await _userRepository.UpdateProfileImageAsync(id, imagePath);
            if (!success)
            throw new ArgumentException("Failed to update profile image.");

            // 5. Atnaujinam user objektą ir grąžinam DTO
            user.ProfileImage = imagePath;

            return _converter.ConvertOne(user, u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                ProfileImage = u.ProfileImage
            });
        }

    }
}