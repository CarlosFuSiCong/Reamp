using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.Orders.Services;
using Reamp.Domain.Common.Abstractions;
using Reamp.Shared;
using System.Security.Claims;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IShootOrderAppService _appService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IShootOrderAppService appService, ILogger<OrdersController> logger)
        {
            _appService = appService;
            _logger = logger;
        }

        // Get current user ID from claims
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID claim in token");
                throw new UnauthorizedAccessException("Invalid user authentication");
            }
            
            return userId;
        }

        // POST /api/orders - Place new order
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _appService.PlaceOrderAsync(dto, currentUserId, ct);

                _logger.LogInformation("Order placed: {OrderId} by user {UserId}", result.Id, currentUserId);

                return CreatedAtAction(
                    nameof(GetOrderDetail),
                    new { id = result.Id },
                    ApiResponse<OrderDetailDto>.Ok(result, "Order placed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to place order");
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when placing order");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when placing order");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/orders - Get order list with pagination
        [HttpGet]
        public async Task<IActionResult> GetOrderList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var pageRequest = new PageRequest(page, pageSize);

                var result = await _appService.GetListAsync(pageRequest, currentUserId, ct);

                return Ok(ApiResponse<IPagedList<OrderListDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order list");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/orders/{id} - Get order detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetail(Guid id, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _appService.GetByIdAsync(id, currentUserId, ct);

                if (result == null)
                    return NotFound(ApiResponse<object>.Fail("Order not found"));

                return Ok(ApiResponse<OrderDetailDto>.Ok(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order detail: {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/orders/{id}/tasks - Add task to order
        [HttpPost("{id}/tasks")]
        public async Task<IActionResult> AddTask(Guid id, [FromBody] AddTaskDto dto, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.AddTaskAsync(id, dto, currentUserId, ct);

                _logger.LogInformation("Task added to order {OrderId}", id);

                return Ok(ApiResponse.Ok("Task added successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding task to order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // DELETE /api/orders/{id}/tasks/{taskId} - Remove task from order
        [HttpDelete("{id}/tasks/{taskId}")]
        public async Task<IActionResult> RemoveTask(Guid id, Guid taskId, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.RemoveTaskAsync(id, taskId, currentUserId, ct);

                _logger.LogInformation("Task {TaskId} removed from order {OrderId}", taskId, id);

                return Ok(ApiResponse.Ok("Task removed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing task from order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/orders/{id}/accept - Accept order
        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptOrder(Guid id, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.AcceptAsync(id, currentUserId, ct);

                _logger.LogInformation("Order accepted: {OrderId}", id);

                return Ok(ApiResponse.Ok("Order accepted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/orders/{id}/schedule - Schedule order
        [HttpPost("{id}/schedule")]
        public async Task<IActionResult> ScheduleOrder(Guid id, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.ScheduleAsync(id, currentUserId, ct);

                _logger.LogInformation("Order scheduled: {OrderId}", id);

                return Ok(ApiResponse.Ok("Order scheduled successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/orders/{id}/start - Start order
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartOrder(Guid id, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.StartAsync(id, currentUserId, ct);

                _logger.LogInformation("Order started: {OrderId}", id);

                return Ok(ApiResponse.Ok("Order started successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/orders/{id}/complete - Complete order
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(Guid id, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.CompleteAsync(id, currentUserId, ct);

                _logger.LogInformation("Order completed: {OrderId}", id);

                return Ok(ApiResponse.Ok("Order completed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/orders/{id}/cancel - Cancel order
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderDto? dto, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _appService.CancelAsync(id, currentUserId, dto?.Reason, ct);

                _logger.LogInformation("Order cancelled: {OrderId}", id);

                return Ok(ApiResponse.Ok("Order cancelled successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to order {OrderId}", id);
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }
    }

    // DTO for cancel order request
    public class CancelOrderDto
    {
        public string? Reason { get; set; }
    }
}

