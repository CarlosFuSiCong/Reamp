using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Infrastructure.Repositories.Common;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class RefreshTokenRepository : EfRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);
        }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(ct);
        }

        public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var tokens = await _dbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.Revoke();
            }
        }
    }
}

