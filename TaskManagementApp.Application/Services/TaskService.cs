using Microsoft.Extensions.Logging;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;
using TaskManagementApp.Infrastructure.Repositories;

namespace TaskManagementApp.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<List<UserTask>> GetAllTasksAsync(TaskFilterRequest? filter)
        {
            try
            {
                return await _taskRepository.GetAllAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                throw;
            }
        }

        public async Task<List<UserTask>> GetTasksByUserIDAsync(long userID)
        {
            try
            {
                return await _taskRepository.GetTasksByUserIDAsync(userID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                throw;
            }
        }

        public async Task<UserTask?> GetTaskByIdAsync(long taskId)
        {
            try
            {
                return await _taskRepository.GetByIdAsync(taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<UserTask> CreateTaskAsync(CreateTaskRequest request, string createdBy)
        {
            try
            {
                var task = new UserTask
                {
                    TaskName = request.TaskName,
                    TaskDescription = request.TaskDescription,
                    AssignedUserIDs = request.AssignedUserIDs,
                    Status = TaskCurrentStatus.ToDo,
                    IsDelayed = false,
                    Deadline = request.Deadline,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = createdBy
                };

                return await _taskRepository.CreateAsync(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<UserTask?> UpdateTaskAsync(long taskId, UpdateTaskRequest request, string updatedBy)
        {
            try
            {
                var existingTask = await _taskRepository.GetByIdAsync(taskId);
                if (existingTask == null)
                {
                    return null;
                }

                existingTask.TaskName = request.TaskName;
                existingTask.TaskDescription = request.TaskDescription;
                existingTask.AssignedUserIDs = request.AssignedUserIDs;
                existingTask.Status = request.Status;
                existingTask.Deadline = request.Deadline;
                existingTask.UpdatedOn = DateTime.UtcNow;
                existingTask.UpdatedBy = updatedBy;

                // Check if task is delayed
                if (existingTask.Deadline.HasValue &&
                    existingTask.Deadline.Value < DateTime.UtcNow &&
                    existingTask.Status != TaskCurrentStatus.Completed)
                {
                    existingTask.IsDelayed = true;
                }

                return await _taskRepository.UpdateAsync(existingTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(long taskId, string deletedBy)
        {
            try
            {
                return await _taskRepository.DeleteAsync(taskId, deletedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<UserTask?> UpdateTaskCurrentStatusAsync(long taskId, TaskCurrentStatus status, string updatedBy)
        {
            try
            {
                return await _taskRepository.UpdateStatusAsync(taskId, status, updatedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status {TaskId}", taskId);
                throw;
            }
        }
    }
}