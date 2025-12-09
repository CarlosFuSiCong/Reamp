using Reamp.Application.Authentication.Dtos;

namespace Reamp.Application.Authentication.Services
{
    // Authentication service interface
    public interface IAuthService
    {
        // Register new user and create profile
        Task<TokenResponse> RegisterAsync(RegisterDto dto, CancellationToken ct = default);

        // Login user and generate token
        Task<TokenResponse> LoginAsync(LoginDto dto, CancellationToken ct = default);

        // Get current user info
        Task<UserInfoDto?> GetUserInfoAsync(Guid userId, CancellationToken ct = default);

        // Update user profile
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken ct = default);

        // Change user password
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default);

        // Refresh access token using refresh token
        Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}

