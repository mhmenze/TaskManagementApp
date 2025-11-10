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
    [ServiceFilter(typeof(AuthenticationFilter))]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        //[Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserTask>>>> GetAll([FromQuery] TaskFilterRequest? filter)
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync(filter);
                return Ok(new ApiResponse<List<UserTask>>
                {
                    Success = true,
                    Message = $"Retrieved {tasks.Count} tasks",
                    Data = tasks
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, new ApiResponse<List<UserTask>>
                {
                    Success = false,
                    Message = "Error retrieving tasks"
                });
            }
        }

        [HttpGet("user/{userId:long}")]
        public async Task<ActionResult<ApiResponse<List<UserTask>>>> GetTasksByUserID(long userID)
        {
            try
            {
                var tasks = await _taskService.GetTasksByUserIDAsync(userID);
                return Ok(new ApiResponse<List<UserTask>>
                {
                    Success = true,
                    Message = $"Retrieved {tasks.Count} tasks",
                    Data = tasks
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, new ApiResponse<List<UserTask>>
                {
                    Success = false,
                    Message = "Error retrieving tasks"
                });
            }
        }

        [HttpGet("{taskId:long}")]
        public async Task<ActionResult<ApiResponse<UserTask>>> GetById(long taskId)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);

                if (task == null)
                {
                    return NotFound(new ApiResponse<UserTask>
                    {
                        Success = false,
                        Message = "Task not found"
                    });
                }

                return Ok(new ApiResponse<UserTask>
                {
                    Success = true,
                    Data = task
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId}", taskId);
                return StatusCode(500, new ApiResponse<UserTask>
                {
                    Success = false,
                    Message = "Error retrieving task"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserTask>>> Create([FromBody] CreateTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<UserTask>
                    {
                        Success = false,
                        Message = "Invalid request",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var username = HttpContext.Session.GetString("Username") ?? "System";
                var task = await _taskService.CreateTaskAsync(request, username);

                return CreatedAtAction(
                    nameof(GetById),
                    new { taskId = task.TaskID },
                    new ApiResponse<UserTask>
                    {
                        Success = true,
                        Message = "Task created successfully",
                        Data = task
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new ApiResponse<UserTask>
                {
                    Success = false,
                    Message = "Error creating task"
                });
            }
        }

        [HttpPut("{taskId:long}")]
        public async Task<ActionResult<ApiResponse<UserTask>>> Update(long taskId, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<UserTask>
                    {
                        Success = false,
                        Message = "Invalid request",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var username = HttpContext.Session.GetString("Username") ?? "System";
                var task = await _taskService.UpdateTaskAsync(taskId, request, username);

                if (task == null)
                {
                    return NotFound(new ApiResponse<UserTask>
                    {
                        Success = false,
                        Message = "Task not found"
                    });
                }

                return Ok(new ApiResponse<UserTask>
                {
                    Success = true,
                    Message = "Task updated successfully",
                    Data = task
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", taskId);
                return StatusCode(500, new ApiResponse<UserTask>
                {
                    Success = false,
                    Message = "Error updating task"
                });
            }
        }

        [HttpDelete("{taskId:long}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(long taskId)
        {
            try
            {
                var username = HttpContext.Session.GetString("Username") ?? "System";
                var success = await _taskService.DeleteTaskAsync(taskId, username);

                if (!success)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task not found"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error deleting task"
                });
            }
        }

        [HttpPatch("{taskId:long}/status")]
        public async Task<ActionResult<ApiResponse<UserTask>>> UpdateStatus(long taskId, [FromBody] TaskCurrentStatus status)
        {
            try
            {
                var username = HttpContext.Session.GetString("Username") ?? "System";
                var task = await _taskService.UpdateTaskCurrentStatusAsync(taskId, status, username);

                if (task == null)
                {
                    return NotFound(new ApiResponse<UserTask>
                    {
                        Success = false,
                        Message = "Task not found"
                    });
                }

                return Ok(new ApiResponse<UserTask>
                {
                    Success = true,
                    Message = "Task status updated successfully",
                    Data = task
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status {TaskId}", taskId);
                return StatusCode(500, new ApiResponse<UserTask>
                {
                    Success = false,
                    Message = "Error updating task status"
                });
            }
        }
    }
}