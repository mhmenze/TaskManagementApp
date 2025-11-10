using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Infrastructure.Repositories
{
    public interface ITaskRepository
    {
        Task<List<UserTask>> GetAllAsync(TaskFilterRequest? filter);
        Task<List<UserTask>> GetTasksByUserIDAsync(long userID);
        Task<UserTask?> GetByIdAsync(long taskId);
        Task<UserTask> CreateAsync(UserTask task);
        Task<UserTask?> UpdateAsync(UserTask task);
        Task<bool> DeleteAsync(long taskId, string deletedBy);
        Task<UserTask?> UpdateStatusAsync(long taskId, TaskCurrentStatus status, string updatedBy);
    }
}