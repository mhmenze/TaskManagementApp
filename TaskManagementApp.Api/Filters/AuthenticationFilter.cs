using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagementApp.Domain.DTOs;

namespace TaskManagementApp.Api.Filters
{
    public class AuthenticationFilter : IAsyncActionFilter
    {
        private readonly ILogger<AuthenticationFilter> _logger;

        public AuthenticationFilter(ILogger<AuthenticationFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userID = context.HttpContext.Session.GetInt32("UserID");
            var username = context.HttpContext.Session.GetString("Username");

            if (userID == null || string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Unauthorized access attempt to {Path}", context.HttpContext.Request.Path);

                context.Result = new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Authentication required. Please login."
                });
                return;
            }

            await next();
        }
    }
}