using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}




