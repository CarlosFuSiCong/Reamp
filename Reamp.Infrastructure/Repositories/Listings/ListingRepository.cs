using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Listings.Repositories;
using Reamp.Infrastructure;
using Reamp.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Listings
{
    public sealed class ListingRepository : BaseRepository<Listing>, IListingRepository
    {
        public ListingRepository(ApplicationDbContext db, ILogger<ListingRepository> logger) : base(db, logger)
        {
        }

        public override async Task<Listing?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(x => x.Id == id, ct);

        public Task UpdateAsync(Listing entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<Listing?> GetAggregateAsync(Guid id, CancellationToken ct = default)
        {
            return await _set
                .Include("_mediaRefs")
                .Include("_agentSnapshots")
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
