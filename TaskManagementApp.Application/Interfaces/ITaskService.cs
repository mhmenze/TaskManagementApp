using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Application.Interfaces
{
    public interface ITaskService
    {
        Task<List<UserTask>> GetAllTasksAsync(TaskFilterRequest? filter);
        Task<List<UserTask>> GetTasksByUserIDAsync(long userID);
        Task<UserTask?> GetTaskByIdAsync(long taskId);
        Task<UserTask> CreateTaskAsync(CreateTaskRequest request, string createdBy);
        Task<UserTask?> UpdateTaskAsync(long taskId, UpdateTaskRequest request, string updatedBy);
        Task<bool> DeleteTaskAsync(long taskId, string deletedBy);
        Task<UserTask?> UpdateTaskCurrentStatusAsync(long taskId, TaskCurrentStatus status, string updatedBy);
    }
}