using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Admin.Dtos;
using Reamp.Application.Admin.Services;
using Reamp.Domain.Accounts.Enums;
using Reamp.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public sealed class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats(CancellationToken ct)
        {
            _logger.LogInformation("Admin stats requested");

            var stats = await _adminService.GetStatsAsync(ct);

            return Ok(ApiResponse<AdminStatsResponse>.Ok(stats));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            _logger.LogInformation("Admin users list requested");

            var users = await _adminService.GetUsersAsync(ct);

            return Ok(ApiResponse<List<AdminUserDto>>.Ok(users));
        }

        [HttpPut("users/{userId}/status")]
        public async Task<IActionResult> UpdateUserStatus(
            Guid userId,
            [FromBody] UpdateUserStatusDto dto,
            CancellationToken ct)
        {
            var adminIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (adminIdClaim == null || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(ApiResponse.Fail("Invalid token"));
            }

            _logger.LogInformation("Admin {AdminId} updating user {UserId} status to {Status}", 
                adminId, userId, dto.Status);

            await _adminService.UpdateUserStatusAsync(userId, dto.Status, ct);

            return Ok(ApiResponse.Ok("User status updated successfully"));
        }

        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(
            Guid userId,
            [FromBody] UpdateUserRoleDto dto,
            CancellationToken ct)
        {
            var adminIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (adminIdClaim == null || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(ApiResponse.Fail("Invalid token"));
            }

            _logger.LogInformation("Admin {AdminId} updating user {UserId} role to {Role}", 
                adminId, userId, dto.Role);

            await _adminService.UpdateUserRoleAsync(userId, dto.Role, ct);

            return Ok(ApiResponse.Ok("User role updated successfully"));
        }
    }
}
