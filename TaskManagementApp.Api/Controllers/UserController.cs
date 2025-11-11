using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Api.Filters;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //[Authorize(Roles = "admin")]
        [HttpGet]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                // Remove passwords from response
                users.ForEach(u => u.Password = string.Empty);

                return Ok(new ApiResponse<List<User>>
                {
                    Success = true,
                    Message = $"Retrieved {users.Count} users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new ApiResponse<List<User>>
                {
                    Success = false,
                    Message = "Error retrieving users"
                });
            }
        }

        [HttpGet("{userId:long}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public async Task<ActionResult<ApiResponse<User>>> GetById(long userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ApiResponse<User>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                user.Password = string.Empty;

                return Ok(new ApiResponse<User>
                {
                    Success = true,
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return StatusCode(500, new ApiResponse<User>
                {
                    Success = false,
                    Message = "Error retrieving user"
                });
            }
        }

        //[Authorize(Roles = "admin")]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<User>>> Register([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<User>
                    {
                        Success = false,
                        Message = "Invalid request",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var currentUsername = User.Identity?.Name ?? "System";

                var user = await _userService.CreateUserAsync(request, currentUsername);

                if (user == null)
                {
                    return BadRequest(new ApiResponse<User>
                    {
                        Success = false,
                        Message = "Username or email already exists"
                    });
                }

                user.Password = string.Empty;

                return CreatedAtAction(
                    nameof(GetById),
                    new { userId = user.UserID },
                    new ApiResponse<User>
                    {
                        Success = true,
                        Message = "User registered successfully",
                        Data = user
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new ApiResponse<User>
                {
                    Success = false,
                    Message = "Error registering user"
                });
            }
        }

        [HttpPut("{userId:long}")]
        public async Task<ActionResult<ApiResponse<User>>> Update(long userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<User>
                    {
                        Success = false,
                        Message = "Invalid request",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                var user = await _userService.UpdateUserAsync(userId, request);

                if (user == null)
                {
                    return NotFound(new ApiResponse<User>
                    {
                        Success = false,
                        Message = "User not found or update failed"
                    });
                }

                user.Password = string.Empty;

                return Ok(new ApiResponse<User>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return StatusCode(500, new ApiResponse<User>
                {
                    Success = false,
                    Message = "Error updating user"
                });
            }
        }

        //[Authorize(Roles = "admin")]
        [HttpDelete("{userId:long}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(long userId, [FromQuery] bool softDelete = true)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(userId, softDelete);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found or could not be deleted"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error deleting user"
                });
            }
        }
    }
}