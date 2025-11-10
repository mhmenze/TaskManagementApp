using System.Data;
using Microsoft.Data.SqlClient;
using TaskManagementApp.Domain;
using TaskManagementApp.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetAllUsers", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(MapToUser(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }

            return users;
        }

        public async Task<User?> GetByIdAsync(long userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetUserById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserID", userId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToUser(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetUserByUsername", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Username", username);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToUser(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username {Username}", username);
                throw;
            }
        }

        public async Task<User?> CreateAsync(User user)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_CreateUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@MiddleName", (object?)user.MiddleName ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@DisplayName", (object?)user.DisplayName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@UserRole", user.UserRole);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@CreatedBy", (object?)user.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)user.UpdatedBy ?? DBNull.Value);

                var outputParam = new SqlParameter("@UserID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                user.UserID = (long)outputParam.Value;
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<User?> UpdateAsync(User user)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_UpdateUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserID", user.UserID);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@MiddleName", (object?)user.MiddleName ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@DisplayName", (object?)user.DisplayName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@UserRole", user.UserRole);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)user.UpdatedBy ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToUser(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", user.UserID);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long userId, bool softDelete)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_DeleteUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@SoftDelete", softDelete);
                var outputParam = new SqlParameter("@Deleted", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return (bool)outputParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Deleting user {userId}", userId);
                throw;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_CheckUsernameExists", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Username", username);

                var outputParam = new SqlParameter("@Exists", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return (bool)outputParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username existence");
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_CheckEmailExists", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Email", email);

                var outputParam = new SqlParameter("@Exists", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return (bool)outputParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence");
                throw;
            }
        }

        private User MapToUser(SqlDataReader reader)
        {
            return new User
            {
                UserID = reader.GetInt64(reader.GetOrdinal("UserID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("MiddleName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("DisplayName")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                UserRole = reader.GetString(reader.GetOrdinal("UserRole")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
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