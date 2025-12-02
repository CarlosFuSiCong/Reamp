using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Shoots.Entities;
using Reamp.Domain.Shoots.Repositories;
using Reamp.Infrastructure.Repositories.Common;

namespace Reamp.Infrastructure.Repositories.Orders
{
    public sealed class ShootOrderRepository : BaseRepository<ShootOrder>, IShootOrderRepository
    {
        public ShootOrderRepository(ApplicationDbContext db, ILogger<ShootOrderRepository> logger) : base(db, logger)
        {
        }

        public override async Task<ShootOrder?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var query = _set.AsQueryable();
            if (asNoTracking) query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<ShootOrder?> GetAggregateAsync(Guid id, CancellationToken ct = default)
        {
            // Load order with all tasks
            return await _set
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<IPagedList<ShootOrder>> ListAsync(
            PageRequest page,
            Guid? agencyId = null,
            Guid? studioId = null,
            Guid? createdBy = null,
            CancellationToken ct = default)
        {
            var query = _set.AsNoTracking().Include(x => x.Tasks).AsQueryable();

            // Apply filters
            if (agencyId.HasValue)
                query = query.Where(x => x.AgencyId == agencyId.Value);

            if (studioId.HasValue)
                query = query.Where(x => x.StudioId == studioId.Value);

            if (createdBy.HasValue)
                query = query.Where(x => x.CreatedBy == createdBy.Value);

            // Order by creation date descending
            query = query.OrderByDescending(x => x.CreatedAtUtc);

            return await ToPagedListAsync(query, page, ct);
        }

        public Task UpdateAsync(ShootOrder entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }
    }
}

