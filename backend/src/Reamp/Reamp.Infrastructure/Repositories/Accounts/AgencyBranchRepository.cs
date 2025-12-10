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
    public sealed class AgencyBranchRepository : BaseRepository<AgencyBranch>, IAgencyBranchRepository
    {
        public AgencyBranchRepository(ApplicationDbContext db, ILogger<AgencyBranchRepository> logger)
            : base(db, logger) { }

        public async Task<AgencyBranch?> GetBySlugAsync(Guid agencyId, Slug slug, bool asNoTracking = true, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(x => x.AgencyId == agencyId && x.Slug.Value == slug.Value, ct);
        }

        public async Task<IPagedList<AgencyBranch>> ListAsync(Guid agencyId, PageRequest page, string? search = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(b => b.AgencyId == agencyId);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(b => b.Name.ToLower().Contains(s) || b.Slug.Value.Contains(s));
            }

            q = q.OrderBy(b => b.Name);
            return await ToPagedListAsync(q, page, ct);
        }

        public async Task<AgencyBranch?> GetByIdAndAgencyAsync(
            Guid id,
            Guid agencyId,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<AgencyBranch> q = _set.Where(b => b.Id == id && b.AgencyId == agencyId && b.DeletedAtUtc == null);
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<int> CountByAgencyAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _set.CountAsync(b => b.AgencyId == agencyId && b.DeletedAtUtc == null, ct);
        }

        public async Task<List<AgencyBranch>> GetByAgencyAsync(
            Guid agencyId,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<AgencyBranch> q = _set.Where(b => b.AgencyId == agencyId && b.DeletedAtUtc == null)
                .OrderBy(b => b.Name);
            if (asNoTracking) q = q.AsNoTracking();
            return await q.ToListAsync(ct);
        }
    }
}