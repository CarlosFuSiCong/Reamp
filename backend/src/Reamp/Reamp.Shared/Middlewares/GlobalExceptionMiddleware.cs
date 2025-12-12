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
                Log.Warning(ex, "Unauthorized access: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex, "Resource not found: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Resource not found"));
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Invalid argument: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Invalid request"));
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning(ex, "Invalid operation: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Invalid operation"));
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

