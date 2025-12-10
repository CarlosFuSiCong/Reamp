using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.UserProfiles.Dtos;
using Reamp.Application.UserProfiles.Services;
using Reamp.Shared;
using System.Security.Claims;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/profiles")]
    [Authorize]
    public sealed class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileAppService _appService;
        private readonly ILogger<UserProfilesController> _logger;

        public UserProfilesController(
            IUserProfileAppService appService,
            ILogger<UserProfilesController> logger)
        {
            _appService = appService;
            _logger = logger;
        }

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id, CancellationToken ct)
        {
            var result = await _appService.GetByIdAsync(id, ct);
            if (result == null)
                return NotFound(ApiResponse<object>.Fail("User profile not found"));

            return Ok(ApiResponse<UserProfileDto>.Ok(result));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _appService.GetByApplicationUserIdAsync(currentUserId, ct);
            
            if (result == null)
                return NotFound(ApiResponse<object>.Fail("User profile not found"));

            return Ok(ApiResponse<UserProfileDto>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(
            Guid id,
            [FromBody] UpdateUserProfileDto dto,
            CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            
            var profile = await _appService.GetByIdAsync(id, ct);
            if (profile == null)
                return NotFound(ApiResponse<object>.Fail("User profile not found"));

            if (profile.ApplicationUserId != currentUserId && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("User {UserId} attempted to update profile {ProfileId} without permission",
                    currentUserId, id);
                return Forbid();
            }

            var result = await _appService.UpdateAsync(id, dto, ct);
            _logger.LogInformation("User profile {ProfileId} updated", id);

            return Ok(ApiResponse<UserProfileDto>.Ok(result, "Profile updated successfully"));
        }

        [HttpPut("{id}/avatar")]
        public async Task<IActionResult> UpdateAvatar(
            Guid id,
            [FromBody] UpdateAvatarDto dto,
            CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();
            
            var profile = await _appService.GetByIdAsync(id, ct);
            if (profile == null)
                return NotFound(ApiResponse<object>.Fail("User profile not found"));

            if (profile.ApplicationUserId != currentUserId && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("User {UserId} attempted to update avatar for profile {ProfileId} without permission",
                    currentUserId, id);
                return Forbid();
            }

            var result = await _appService.UpdateAvatarAsync(id, dto.AvatarAssetId, ct);
            _logger.LogInformation("User profile {ProfileId} avatar updated", id);

            return Ok(ApiResponse<UserProfileDto>.Ok(result, "Avatar updated successfully"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProfiles(
            [FromQuery] string? keyword,
            [FromQuery] int limit = 20,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Ok(ApiResponse<List<UserProfileDto>>.Ok(new List<UserProfileDto>()));

            var results = await _appService.SearchAsync(keyword, limit, ct);
            return Ok(ApiResponse<List<UserProfileDto>>.Ok(results));
        }
    }
}

