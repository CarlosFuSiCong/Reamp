using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

        public AuthController(
            IAuthService authService, 
            ILogger<AuthController> logger,
            UserManager<Infrastructure.Identity.ApplicationUser> userManager)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
        }

        // Get password policy
        [HttpGet("password-policy")]
        public IActionResult GetPasswordPolicy()
        {
            var options = _userManager.Options.Password;
            var policy = new PasswordPolicyDto
            {
                RequiredLength = options.RequiredLength,
                RequireNonAlphanumeric = options.RequireNonAlphanumeric,
                RequireDigit = options.RequireDigit,
                RequireLowercase = options.RequireLowercase,
                RequireUppercase = options.RequireUppercase
            };

            return Ok(ApiResponse<PasswordPolicyDto>.Ok(policy));
        }

        // Register new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            _logger.LogInformation("User registration attempt for email: {Email}", dto.Email);

            var response = await _authService.RegisterAsync(dto, ct);

            Guid userId;
            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(response.AccessToken);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out userId))
                {
                    _logger.LogError("Invalid token generated during registration for email: {Email}", dto.Email);
                    return StatusCode(500, ApiResponse.Fail("Registration succeeded but failed to process authentication token"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decode token during registration for email: {Email}", dto.Email);
                return StatusCode(500, ApiResponse.Fail("Registration succeeded but failed to decode authentication token"));
            }

            UserInfoDto userInfo;
            try
            {
                userInfo = await _authService.GetUserInfoAsync(userId, ct);
                if (userInfo == null)
                {
                    _logger.LogError("User info not found after registration for userId: {UserId}", userId);
                    return StatusCode(500, ApiResponse.Fail("Registration succeeded but failed to retrieve user information"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user info during registration for userId: {UserId}", userId);
                return StatusCode(500, ApiResponse.Fail("Registration succeeded but failed to retrieve user information"));
            }

            SetTokenCookies(response);

            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            return Ok(ApiResponse<UserInfoDto>.Ok(userInfo, "Registration successful"));
        }

        // Login user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var response = await _authService.LoginAsync(dto, ct);

            Guid userId;
            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(response.AccessToken);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out userId))
                {
                    _logger.LogError("Invalid token generated during login for email: {Email}", dto.Email);
                    return StatusCode(500, ApiResponse.Fail("Login succeeded but failed to process authentication token"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decode token during login for email: {Email}", dto.Email);
                return StatusCode(500, ApiResponse.Fail("Login succeeded but failed to decode authentication token"));
            }

            UserInfoDto userInfo;
            try
            {
                userInfo = await _authService.GetUserInfoAsync(userId, ct);
                if (userInfo == null)
                {
                    _logger.LogError("User info not found after login for userId: {UserId}", userId);
                    return StatusCode(500, ApiResponse.Fail("Login succeeded but failed to retrieve user information"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user info during login for userId: {UserId}", userId);
                return StatusCode(500, ApiResponse.Fail("Login succeeded but failed to retrieve user information"));
            }

            SetTokenCookies(response);

            _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
            return Ok(ApiResponse<UserInfoDto>.Ok(userInfo, "Login successful"));
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

        // Refresh access token
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(CancellationToken ct)
        {
            _logger.LogInformation("Token refresh attempt");

            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(ApiResponse.Fail("Refresh token not found"));
            }

            var response = await _authService.RefreshTokenAsync(refreshToken, ct);

            SetTokenCookies(response);

            _logger.LogInformation("Token refreshed successfully");
            return Ok(ApiResponse.Ok("Token refreshed successfully"));
        }

        // Logout user
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            ClearTokenCookies();
            _logger.LogInformation("User logged out successfully");
            return Ok(ApiResponse.Ok("Logout successful"));
        }

        private void SetTokenCookies(TokenResponse tokenResponse)
        {
            var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development";

            Response.Cookies.Append("accessToken", tokenResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
                Expires = tokenResponse.ExpiresAt,
                Path = "/"
            });

            Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            });
        }

        private void ClearTokenCookies()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
        }
    }
}
