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
    public sealed class StudioRepository : BaseRepository<Studio>, IStudioRepository
    {
        public StudioRepository(ApplicationDbContext db, ILogger<StudioRepository> logger)
            : base(db, logger) { }

        public async Task<Studio?> GetBySlugAsync(Slug slug, bool asNoTracking = true, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(x => x.Slug.Value == slug.Value, ct);
        }

        public Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken ct = default)
            => _set.AnyAsync(x => x.Slug.Value == slug.Value, ct);

        public async Task<IPagedList<Studio>> ListAsync(PageRequest page, string? search = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(st => st.Name.ToLower().Contains(s) || st.Slug.Value.Contains(s));
            }

            q = q.OrderBy(st => st.Name);
            return await ToPagedListAsync(q, page, ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await _set.AnyAsync(s => s.Id == id && s.DeletedAtUtc == null, ct);
        }
    }
}