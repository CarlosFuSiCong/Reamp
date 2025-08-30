using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(ApplicationDbContext db, ILogger<UserProfileRepository> logger)
            : base(db, logger) { }

        public async Task<UserProfile?> GetByIdAsync(
            Guid id,
            bool includeDeleted = false,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<UserProfile> q = _set;
            if (includeDeleted) q = q.IgnoreQueryFilters();
            if (asNoTracking) q = q.AsNoTracking();

            return await q.FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<UserProfile?> GetByApplicationUserIdAsync(
            Guid appUserId,
            bool includeDeleted = false,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<UserProfile> q = _set;
            if (includeDeleted) q = q.IgnoreQueryFilters();
            if (asNoTracking) q = q.AsNoTracking();

            return await q.FirstOrDefaultAsync(p => p.ApplicationUserId == appUserId, ct);
        }

        public Task<bool> ExistsByApplicationUserIdAsync(
            Guid appUserId,
            CancellationToken ct = default)
        {
            return _set.IgnoreQueryFilters().AnyAsync(p => p.ApplicationUserId == appUserId, ct);
        }
    }
}