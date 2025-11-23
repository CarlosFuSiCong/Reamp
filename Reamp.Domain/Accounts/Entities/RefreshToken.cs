using Reamp.Domain.Common.Entities;

namespace Reamp.Domain.Accounts.Entities
{
    // Refresh token entity for token rotation
    public sealed class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = default!;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? ReplacedByToken { get; private set; }

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, int expiryDays)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required", nameof(userId));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required", nameof(token));

            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
            };
        }

        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

        public void Revoke(string? replacedByToken = null)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
        }
    }
}

