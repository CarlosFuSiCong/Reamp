using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.Orders.Services;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Orders.Enums;
using Reamp.Shared;
using System.IdentityModel.Tokens.Jwt;
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

        public OrdersController(
            IShootOrderAppService appService,
            ILogger<OrdersController> logger)
        {
            _appService = appService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID claim in token");
                throw new UnauthorizedAccessException("Invalid user authentication");
            }
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _appService.PlaceOrderAsync(dto, currentUserId, ct);

            return CreatedAtAction(
                nameof(GetOrderDetail),
                new { id = result.Id },
                ApiResponse<OrderDetailDto>.Ok(result, "Order placed successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderList(
            [FromQuery] Guid? agencyId = null,
            [FromQuery] Guid? studioId = null,
            [FromQuery] Guid? listingId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] ShootOrderStatus? status = null,
            [FromQuery] string? keyword = null,
            CancellationToken ct = default)
        {
            var currentUserId = GetCurrentUserId();
            var pageRequest = new PageRequest(page, pageSize);

            // Create filter from query parameters
            var filter = new OrderFilterDto
            {
                AgencyId = agencyId,
                StudioId = studioId,
                ListingId = listingId,
                Status = status
                // keyword filtering is not yet implemented in the backend
            };

            var result = await _appService.GetFilteredListAsync(filter, pageRequest, currentUserId, ct);

            return Ok(ApiResponse<IPagedList<OrderListDto>>.Ok(result));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderDetail(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _appService.GetByIdAsync(id, currentUserId, ct);

            if (result == null)
                return NotFound(ApiResponse<object>.Fail("Order not found"));

            return Ok(ApiResponse<OrderDetailDto>.Ok(result));
        }

        [HttpPost("{id:guid}/tasks")]
        public async Task<IActionResult> AddTask(Guid id, [FromBody] AddTaskDto dto, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.AddTaskAsync(id, dto, currentUserId, ct);

            return Ok(ApiResponse.Ok("Task added successfully"));
        }

        [HttpDelete("{id}/tasks/{taskId}")]
        public async Task<IActionResult> RemoveTask(Guid id, Guid taskId, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.RemoveTaskAsync(id, taskId, currentUserId, ct);

            return Ok(ApiResponse.Ok("Task removed successfully"));
        }

        [HttpPost("{id:guid}/accept")]
        public async Task<IActionResult> AcceptOrder(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.AcceptAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Order accepted successfully"));
        }

        [HttpPost("{id:guid}/schedule")]
        public async Task<IActionResult> ScheduleOrder(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.ScheduleAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Order scheduled successfully"));
        }

        [HttpPost("{id:guid}/start")]
        public async Task<IActionResult> StartOrder(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.StartAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Order started successfully"));
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> CompleteOrder(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.CompleteAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Order completed successfully"));
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderDto? dto, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.CancelAsync(id, currentUserId, dto?.Reason, ct);

            return Ok(ApiResponse.Ok("Order cancelled successfully"));
        }

        [HttpPost("{id:guid}/assign-photographer")]
        public async Task<IActionResult> AssignPhotographer(Guid id, [FromBody] AssignPhotographerDto dto, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.AssignPhotographerAsync(id, dto, currentUserId, ct);

            return Ok(ApiResponse.Ok("Photographer assigned successfully"));
        }

        [HttpDelete("{id:guid}/photographer")]
        public async Task<IActionResult> UnassignPhotographer(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.UnassignPhotographerAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Photographer unassigned successfully"));
        }

        [HttpGet("{id:guid}/available-photographers")]
        public async Task<IActionResult> GetAvailablePhotographers(Guid id, CancellationToken ct)
        {
            var photographers = await _appService.GetAvailablePhotographersAsync(id, ct);

            return Ok(ApiResponse<IReadOnlyList<Application.Read.Staff.DTOs.StaffSummaryDto>>.Ok(photographers));
        }

        [HttpPost("{id:guid}/set-schedule")]
        public async Task<IActionResult> SetSchedule(Guid id, [FromBody] SetScheduleDto dto, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.SetScheduleAsync(id, dto, currentUserId, ct);

            return Ok(ApiResponse.Ok("Schedule set successfully"));
        }

        [HttpDelete("{id:guid}/schedule")]
        public async Task<IActionResult> ClearSchedule(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.ClearScheduleAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Schedule cleared successfully"));
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredList(
            [FromQuery] OrderFilterDto filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var currentUserId = GetCurrentUserId();
            var pageRequest = new PageRequest(page, pageSize);

            var result = await _appService.GetFilteredListAsync(filter, pageRequest, currentUserId, ct);

            return Ok(ApiResponse<IPagedList<OrderListDto>>.Ok(result));
        }

        /// <summary>
        /// Get available orders for photographers to accept (marketplace orders)
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var currentUserId = GetCurrentUserId();
            var pageRequest = new PageRequest(page, pageSize);

            var result = await _appService.GetAvailableOrdersAsync(pageRequest, currentUserId, ct);

            return Ok(ApiResponse<IPagedList<OrderListDto>>.Ok(result));
        }

        /// <summary>
        /// Get orders assigned to the current photographer
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetPhotographerOrders(
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var currentUserId = GetCurrentUserId();
            var pageRequest = new PageRequest(page, pageSize);

            Reamp.Domain.Orders.Enums.ShootOrderStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Reamp.Domain.Orders.Enums.ShootOrderStatus>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            var result = await _appService.GetPhotographerOrdersAsync(pageRequest, currentUserId, statusEnum, ct);

            return Ok(ApiResponse<IPagedList<OrderListDto>>.Ok(result));
        }

        /// <summary>
        /// Accept an available order as a photographer (grab/claim order)
        /// </summary>
        [HttpPost("{id:guid}/accept-photographer")]
        public async Task<IActionResult> AcceptAsPhotographer(Guid id, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            await _appService.AcceptOrderAsPhotographerAsync(id, currentUserId, ct);

            return Ok(ApiResponse.Ok("Order accepted successfully"));
        }
    }
}
