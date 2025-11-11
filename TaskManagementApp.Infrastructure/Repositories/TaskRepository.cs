using System.Data;
using Microsoft.Data.SqlClient;
using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;
using TaskManagementApp.Application.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TaskManagementApp.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(string connectionString, ILogger<TaskRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<List<UserTask>> GetAllAsync(TaskFilterRequest? filter)
        {
            var tasks = new List<UserTask>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetAllTasks", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (filter != null)
                {
                    command.Parameters.AddWithValue("@Status",
                        filter.Status.HasValue ? (int)filter.Status.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedUserID",
                        (object?)filter.AssignedUserID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsDelayed",
                        (object?)filter.IsDelayed ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SearchTerm",
                        (object?)filter.SearchTerm ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SortBy",
                        (object?)filter.SortBy ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SortDescending", filter.SortDescending);
                }

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tasks.Add(MapToTask(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                throw;
            }

            return tasks;
        }

        public async Task<List<UserTask>> GetTasksByUserIDAsync(long userID)
        {
            var tasks = new List<UserTask>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetTasksByUserID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (userID != 0)
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                }

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        tasks.Add(MapToTask(reader));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                throw;
            }

            return tasks;
        }

        public async Task<UserTask?> GetByIdAsync(long taskId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetTaskById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TaskID", taskId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToTask(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<UserTask> CreateAsync(UserTask task)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_CreateTask", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TaskName", task.TaskName);
                command.Parameters.AddWithValue("@TaskDescription", (object?)task.TaskDescription ?? DBNull.Value);
                command.Parameters.AddWithValue("@AssignedUserIDs",
                    task.AssignedUserIDs != null && task.AssignedUserIDs.Any()
                        ? JsonSerializer.Serialize(task.AssignedUserIDs)
                        : DBNull.Value);
                command.Parameters.AddWithValue("@Status", (int)task.Status);
                command.Parameters.AddWithValue("@IsDelayed", task.IsDelayed);
                command.Parameters.AddWithValue("@Deadline", (object?)task.Deadline ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", (object?)task.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)task.UpdatedBy ?? DBNull.Value);

                var outputParam = new SqlParameter("@TaskID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                task.TaskID = (long)outputParam.Value;
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<UserTask?> UpdateAsync(UserTask task)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_UpdateTask", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TaskID", task.TaskID);
                command.Parameters.AddWithValue("@TaskName", task.TaskName);
                command.Parameters.AddWithValue("@TaskDescription", (object?)task.TaskDescription ?? DBNull.Value);
                command.Parameters.AddWithValue("@AssignedUserIDs",
                    task.AssignedUserIDs != null && task.AssignedUserIDs.Any()
                        ? JsonSerializer.Serialize(task.AssignedUserIDs)
                        : DBNull.Value);
                command.Parameters.AddWithValue("@Status", (int)task.Status);
                command.Parameters.AddWithValue("@IsDelayed", task.IsDelayed);
                command.Parameters.AddWithValue("@Deadline", (object?)task.Deadline ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)task.UpdatedBy ?? DBNull.Value);

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected != 0 ? task : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", task.TaskID);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long taskId, string deletedBy)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_DeleteTask", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TaskID", taskId);
                command.Parameters.AddWithValue("@DeletedBy", (object?)deletedBy ?? DBNull.Value);

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected != 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<UserTask?> UpdateStatusAsync(long taskId, TaskCurrentStatus status, string updatedBy)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_UpdateTaskStatus", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TaskID", taskId);
                command.Parameters.AddWithValue("@Status", (int)status);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected != 0)
                {
                    return await GetByIdAsync(taskId);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status {TaskId}", taskId);
                throw;
            }
        }

        private UserTask MapToTask(SqlDataReader reader)
        {
            var assignedUserIDsJson = reader.IsDBNull(reader.GetOrdinal("AssignedUserIDs"))
                ? null
                : reader.GetString(reader.GetOrdinal("AssignedUserIDs"));

            return new UserTask
            {
                TaskID = reader.GetInt64(reader.GetOrdinal("TaskID")),
                TaskName = reader.GetString(reader.GetOrdinal("TaskName")),
                TaskDescription = reader.IsDBNull(reader.GetOrdinal("TaskDescription"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("TaskDescription")),
                AssignedUserIDs = !string.IsNullOrEmpty(assignedUserIDsJson)
                    ? JsonSerializer.Deserialize<List<long>>(assignedUserIDsJson)
                    : null,
                Status = (TaskCurrentStatus)reader.GetInt32(reader.GetOrdinal("Status")),
                IsDelayed = reader.GetBoolean(reader.GetOrdinal("IsDelayed")),
                Deadline = reader.IsDBNull(reader.GetOrdinal("Deadline"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("Deadline")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedOn = reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}