using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(long userId);
        Task<User?> CreateUserAsync(CreateUserRequest request, string currentUsername);
        Task<User?> UpdateUserAsync(long userId, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(long userId, bool softDelete);
    }
}