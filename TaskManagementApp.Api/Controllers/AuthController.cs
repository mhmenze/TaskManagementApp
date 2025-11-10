using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Invalid request",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var result = await _authService.AuthenticateAsync(request.Username, request.Password);

                if (!result.Success)
                {
                    return Unauthorized(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = result
                    });
                }

                HttpContext.Session.SetInt32("UserID", (int)result.UserID);
                HttpContext.Session.SetString("Username", result.Username);

                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logged out successfully"
            });
        }

        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            var username = HttpContext.Session.GetString("Username");

            if (userID == null || string.IsNullOrEmpty(username))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No active session"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { UserID = userID.Value, Username = username }
            });
        }

        //[HttpPost("login")]
        //public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(new ApiResponse<LoginResponse>
        //            {
        //                Success = false,
        //                Message = "Invalid request",
        //                Errors = ModelState.Values
        //                    .SelectMany(v => v.Errors)
        //                    .Select(e => e.ErrorMessage)
        //                    .ToList()
        //            });
        //        }

        //        var result = await _authService.AuthenticateAsync(request.Username, request.Password);

        //        if (!result.Success)
        //        {
        //            return Unauthorized(new ApiResponse<LoginResponse>
        //            {
        //                Success = false,
        //                Message = result.Message,
        //                Data = result
        //            });
        //        }
        //        var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, result.Username),
        //            new Claim(ClaimTypes.Role, result.UserRole),
        //            new Claim(ClaimTypes.NameIdentifier, result.UserID.ToString())
        //        };

        //        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(identity);

        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        //        return Ok(new ApiResponse<LoginResponse>
        //        {
        //            Success = true,
        //            Message = "Login successful",
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during login");
        //        return StatusCode(500, new ApiResponse<LoginResponse>
        //        {
        //            Success = false,
        //            Message = "An error occurred during login"
        //        });
        //    }
        //}

        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    HttpContext.Session.Clear();

        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Logged out successfully"
        //    });
        //}

        //[Authorize(Roles = "admin")]
        //[HttpGet("admin-only")]
        //public IActionResult AdminOnly()
        //{
        //    return Ok(new
        //    {
        //        Username = User.Identity?.Name,
        //        Role = User.FindFirst(ClaimTypes.Role)?.Value
        //    });
        //}

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<RegisterResponse>
                    {
                        Success = false,
                        Message = "Invalid input",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                request.CreatedBy = User.Identity?.Name ?? "System";

                var result = await _authService.RegisterAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ApiResponse<RegisterResponse>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = result
                    });
                }

                return Ok(new ApiResponse<RegisterResponse>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "An error occurred during registration"
                });
            }
        }

    }
}