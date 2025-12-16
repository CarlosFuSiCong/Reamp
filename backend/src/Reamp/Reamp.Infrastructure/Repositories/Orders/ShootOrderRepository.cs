using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Orders.Entities;
using Reamp.Domain.Orders.Enums;
using Reamp.Domain.Orders.Repositories;
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

        public async Task<ShootOrder?> GetAggregateAsync(Guid id, bool asNoTracking = false, CancellationToken ct = default)
        {
            // Load order with all tasks
            IQueryable<ShootOrder> query = _set.Include(x => x.Tasks);
            
            if (asNoTracking)
                query = query.AsNoTracking();
            
            return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<IPagedList<ShootOrder>> ListAsync(
            PageRequest page,
            Guid? agencyId = null,
            Guid? studioId = null,
            Guid? createdBy = null,
            CancellationToken ct = default)
        {
            // Build query with AsNoTracking first, then Include
            var query = _set
                .AsNoTracking()
                .Include(x => x.Tasks)
                .AsQueryable();

            // Apply filters
            if (agencyId.HasValue)
                query = query.Where(x => x.AgencyId == agencyId.Value);

            if (studioId.HasValue)
                query = query.Where(x => x.StudioId == studioId.Value);

            if (createdBy.HasValue)
                query = query.Where(x => x.CreatedBy == createdBy.Value);

            // Order by creation date descending
            query = query.OrderByDescending(x => x.CreatedAtUtc);

            // Get paginated result
            return await ToPagedListAsync(query, page, ct);
        }

        public async Task<IPagedList<ShootOrder>> ListFilteredAsync(
            PageRequest page,
            Guid? agencyId = null,
            Guid? studioId = null,
            Guid? listingId = null,
            Guid? photographerId = null,
            ShootOrderStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            Guid? createdBy = null,
            CancellationToken ct = default)
        {
            var query = _set.AsNoTracking().AsQueryable();

            if (agencyId.HasValue)
                query = query.Where(x => x.AgencyId == agencyId.Value);

            if (studioId.HasValue)
                query = query.Where(x => x.StudioId == studioId.Value);

            if (listingId.HasValue)
                query = query.Where(x => x.ListingId == listingId.Value);

            if (photographerId.HasValue)
                query = query.Where(x => x.AssignedPhotographerId == photographerId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (dateFrom.HasValue)
                query = query.Where(x => x.CreatedAtUtc >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.CreatedAtUtc <= dateTo.Value);

            if (createdBy.HasValue)
                query = query.Where(x => x.CreatedBy == createdBy.Value);

            query = query.OrderByDescending(x => x.CreatedAtUtc);

            var pagedResult = await ToPagedListAsync(query, page, ct);

            if (pagedResult.Items.Any())
            {
                var orderIds = pagedResult.Items.Select(x => x.Id).ToList();
                var ordersWithTasks = await _set
                    .AsNoTracking()
                    .Include(x => x.Tasks)
                    .Where(x => orderIds.Contains(x.Id))
                    .ToListAsync(ct);

                var ordersDict = ordersWithTasks.ToDictionary(x => x.Id);
                var orderedItems = pagedResult.Items
                    .Select(x => ordersDict.TryGetValue(x.Id, out var order) ? order : x)
                    .ToList();

                return new PagedList<ShootOrder>(orderedItems, pagedResult.TotalCount, pagedResult.Page, pagedResult.PageSize);
            }

            return pagedResult;
        }

        public Task UpdateAsync(ShootOrder entity, CancellationToken ct = default)
        {
            // For detached entities, use Update which will mark the entity and
            // all its navigation properties with appropriate states
            var entry = _db.Entry(entity);
            
            if (entry.State == EntityState.Detached)
            {
                _set.Update(entity);
            }
            // If already tracked, EF will handle it automatically
            
            return Task.CompletedTask;
        }
        
        public Task MarkOrderUnchangedAsync(ShootOrder entity, CancellationToken ct = default)
        {
            // Mark the order itself as Unchanged to prevent UPDATE
            var orderEntry = _db.Entry(entity);
            orderEntry.State = EntityState.Unchanged;
            
            // Mark all new tasks as Added (they will be in Detached state after AddTask)
            foreach (var task in entity.Tasks)
            {
                var taskEntry = _db.Entry(task);
                if (taskEntry.State == EntityState.Detached)
                {
                    taskEntry.State = EntityState.Added;
                }
            }
            
            return Task.CompletedTask;
        }
    }
}

