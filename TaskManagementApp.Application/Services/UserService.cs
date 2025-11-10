using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;
using TaskManagementApp.Infrastructure.Repositories;

namespace TaskManagementApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            try
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                throw;
            }
        }

        public async Task<User?> CreateUserAsync(CreateUserRequest request, string currentUsername)
        {
            try
            {
                // Check if username already exists
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    _logger.LogWarning("Username already exists: {Username}", request.Username);
                    return null;
                }

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("Email already exists: {Email}", request.Email);
                    return null;
                }

                var user = new User
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    DisplayName = $"{request.FirstName} {request.LastName}",
                    Username = request.Username,
                    Password = HashPassword(request.Password),
                    UserRole = request.UserRole,
                    Email = request.Email,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = currentUsername,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = currentUsername
                };

                return await _userRepository.CreateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(long userId, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            user.FirstName = request.FirstName ?? user.FirstName;
            user.MiddleName = request.MiddleName ?? user.MiddleName;
            user.LastName = request.LastName ?? user.LastName;
            user.DisplayName = request.DisplayName ?? user.DisplayName;
            user.Username = request.Username ?? user.Username;
            if (!string.IsNullOrEmpty(request.Password)) user.Password = HashPassword(request.Password);
            user.UserRole = request.UserRole ?? user.UserRole;
            user.Email = request.Email ?? user.Email;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = request.Username ?? user.UpdatedBy;

            return await _userRepository.UpdateAsync(user);
        }
        public async Task<bool> DeleteUserAsync(long userId, bool softDelete)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            return await _userRepository.DeleteAsync(userId, softDelete);
        }


        private string HashPassword(string password)
        {
            // For now, storing as plain text (NOT RECOMMENDED FOR PRODUCTION)
            // In production, use: BCrypt.Net.BCrypt.HashPassword(password)
            return password;
        }
    }
}