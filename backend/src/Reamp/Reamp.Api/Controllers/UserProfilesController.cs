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

        // GET /api/profiles/{id} - Get user profile by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _appService.GetByIdAsync(id, ct);
                if (result == null)
                    return NotFound(ApiResponse<object>.Fail("User profile not found"));

                return Ok(ApiResponse<UserProfileDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile: {ProfileId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/profiles/me - Get current user's profile
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _appService.GetByApplicationUserIdAsync(currentUserId, ct);
                
                if (result == null)
                    return NotFound(ApiResponse<object>.Fail("User profile not found"));

                return Ok(ApiResponse<UserProfileDto>.Ok(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to profile");
                return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user profile");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // PUT /api/profiles/{id} - Update user profile
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(
            Guid id,
            [FromBody] UpdateUserProfileDto dto,
            CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Check if user is updating their own profile or is admin
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized profile update attempt");
                return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating profile");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {ProfileId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // PUT /api/profiles/{id}/avatar - Update user avatar
        [HttpPut("{id}/avatar")]
        public async Task<IActionResult> UpdateAvatar(
            Guid id,
            [FromBody] UpdateAvatarDto dto,
            CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Check if user is updating their own profile or is admin
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized avatar update attempt");
                return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating avatar");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user avatar: {ProfileId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/profiles/search - Search users by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchProfiles(
            [FromQuery] string? keyword,
            [FromQuery] int limit = 20,
            CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return Ok(ApiResponse<List<UserProfileDto>>.Ok(new List<UserProfileDto>()));

                var results = await _appService.SearchAsync(keyword, limit, ct);
                return Ok(ApiResponse<List<UserProfileDto>>.Ok(results));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching user profiles");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }
    }
}

