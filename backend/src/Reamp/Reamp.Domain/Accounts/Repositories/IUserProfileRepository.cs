using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IUserProfileRepository : IRepository<UserProfile>
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

        Task<List<UserProfile>> SearchAsync(
            string keyword,
            int limit = 20,
            CancellationToken ct = default);
    }
}