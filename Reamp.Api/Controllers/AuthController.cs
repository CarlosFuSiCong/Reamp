using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Authentication.Dtos;
using Reamp.Application.Authentication.Services;
using Reamp.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // Register new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            _logger.LogInformation("User registration attempt for email: {Email}", dto.Email);

            var response = await _authService.RegisterAsync(dto, ct);

            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            return Ok(ApiResponse<TokenResponse>.Ok(response, "Registration successful"));
        }

        // Login user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var response = await _authService.LoginAsync(dto, ct);

            _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
            return Ok(ApiResponse<TokenResponse>.Ok(response, "Login successful"));
        }

        // Get current user info (requires authentication)
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse.Fail("Invalid token"));
            }

            var userInfo = await _authService.GetUserInfoAsync(userId, ct);
            if (userInfo == null)
            {
                return NotFound(ApiResponse.Fail("User not found"));
            }

            return Ok(ApiResponse<UserInfoDto>.Ok(userInfo));
        }

        // Update user profile (requires authentication)
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse.Fail("Invalid token"));
            }

            await _authService.UpdateProfileAsync(userId, dto, ct);

            _logger.LogInformation("User profile updated: {UserId}", userId);
            return Ok(ApiResponse.Ok("Profile updated successfully"));
        }

        // Change password (requires authentication)
        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse.Fail("Invalid token"));
            }

            await _authService.ChangePasswordAsync(userId, dto, ct);

            _logger.LogInformation("Password changed for user: {UserId}", userId);
            return Ok(ApiResponse.Ok("Password changed successfully"));
        }
    }
}

