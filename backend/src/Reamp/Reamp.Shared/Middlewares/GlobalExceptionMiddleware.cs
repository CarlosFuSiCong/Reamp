using Microsoft.AspNetCore.Http;
using Serilog;

namespace Reamp.Shared.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Invalid request"));
            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Operation conflict"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail("An unexpected error occurred", "Internal server error"));
            }
        }
    }
}

