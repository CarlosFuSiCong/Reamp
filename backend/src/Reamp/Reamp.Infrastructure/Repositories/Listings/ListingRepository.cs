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

        public async Task<Listing?> GetByIdAsync(Guid id, bool asNoTracking = true, bool includeDeleted = false, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (includeDeleted) q = q.IgnoreQueryFilters();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(x => x.Id == id, ct);

        public Task UpdateAsync(Listing entity, CancellationToken ct = default)
        {
            // Attach the aggregate root if not tracked
            var entry = _db.Entry(entity);
            
            if (entry.State == EntityState.Detached)
            {
                _set.Attach(entity);
                entry.State = EntityState.Modified;
            }

            // Manually track child entity states for media refs and agent snapshots
            // EF needs to know which children are new (Added) vs existing (Modified)
            
            // Handle media refs
            foreach (var mediaRef in entity.MediaRefs)
            {
                var mediaEntry = _db.Entry(mediaRef);
                if (mediaEntry.State == EntityState.Detached)
                {
                    mediaEntry.State = EntityState.Added;
                }
            }

            // Handle agent snapshots
            foreach (var agentSnapshot in entity.AgentSnapshots)
            {
                var agentEntry = _db.Entry(agentSnapshot);
                if (agentEntry.State == EntityState.Detached)
                {
                    agentEntry.State = EntityState.Added;
                }
            }

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
