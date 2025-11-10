using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(long userId);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> CreateAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeleteAsync(long userId, bool softDelete);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
