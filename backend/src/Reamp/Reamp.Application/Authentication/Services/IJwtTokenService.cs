using Reamp.Application.Authentication.Dtos;

namespace Reamp.Application.Authentication.Services
{
    // JWT token generation and validation service
    public interface IJwtTokenService
    {
        // Generate access token for user
        string GenerateAccessToken(Guid userId, string email, string role);

        // Generate refresh token
        string GenerateRefreshToken();

        // Validate token and extract user ID
        Guid? ValidateToken(string token);

        // Generate token response (access + refresh)
        TokenResponse GenerateTokenResponse(Guid userId, string email, string role);
    }
}

