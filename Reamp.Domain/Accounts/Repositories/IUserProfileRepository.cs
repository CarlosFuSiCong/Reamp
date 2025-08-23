using Reamp.Domain.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByIdAsync(
            Guid id,
            bool includeDeleted = false,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<UserProfile?> GetByApplicationUserIdAsync(
            Guid appUserId,
            bool includeDeleted = false,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<bool> ExistsByApplicationUserIdAsync(
            Guid appUserId,
            CancellationToken ct = default);

        Task AddAsync(UserProfile entity, CancellationToken ct = default);
        Task UpdateAsync(UserProfile entity, CancellationToken ct = default);

        Task SoftDeleteAsync(UserProfile entity, CancellationToken ct = default);
    }
}