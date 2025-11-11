using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;
using TaskManagementApp.Infrastructure.Repositories;

namespace TaskManagementApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<LoginResponse> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                if (!VerifyPassword(password, user.Password))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                return new LoginResponse
                {
                    Success = true,
                    UserID = user.UserID,
                    UserRole = user.UserRole,
                    Username = user.Username,
                    DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {Username}", username);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication"
                };
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    DisplayName = request.DisplayName,
                    Username = request.Username,
                    Password = hashedPassword,
                    Email = request.Email,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                var newUser = await _userRepository.CreateAsync(user);

                return new RegisterResponse
                {
                    Success = true,
                    Message = "User registered successfully",
                    UserID = newUser?.UserID
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", request.Username);
                return new RegisterResponse
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
        }
    }
}
