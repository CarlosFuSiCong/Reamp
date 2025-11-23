namespace Reamp.Application.Authentication
{
    // JWT configuration settings
    public sealed class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        // Secret key for signing tokens (min 32 chars recommended)
        public string SecretKey { get; set; } = default!;

        // Token issuer (e.g. "Reamp.Api")
        public string Issuer { get; set; } = default!;

        // Token audience (e.g. "Reamp.Client")
        public string Audience { get; set; } = default!;

        // Access token expiration in minutes (default 60)
        public int AccessTokenExpirationMinutes { get; set; } = 60;

        // Refresh token expiration in days (default 7)
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}

