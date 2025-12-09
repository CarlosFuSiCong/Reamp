using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.ValueObjects;
using Reamp.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class AgencyRepository : BaseRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository(ApplicationDbContext db, ILogger<AgencyRepository> logger)
            : base(db, logger) { }

        public async Task<Agency?> GetBySlugAsync(Slug slug, bool asNoTracking = true, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(x => x.Slug.Value == slug.Value, ct);
        }

        public Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken ct = default)
            => _set.AnyAsync(x => x.Slug.Value == slug.Value, ct);

        public async Task<IPagedList<Agency>> ListAsync(PageRequest page, string? search = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(a => a.Name.ToLower().Contains(s) || a.Slug.Value.Contains(s));
            }

            q = q.OrderBy(a => a.Name);
            return await ToPagedListAsync(q, page, ct);
        }

        public async Task<IReadOnlyList<AgencyBranch>> ListBranchesAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _db.Set<AgencyBranch>()
                .AsNoTracking()
                .Where(b => b.AgencyId == agencyId)
                .OrderBy(b => b.Name)
                .ToListAsync(ct);
        }
    }
}