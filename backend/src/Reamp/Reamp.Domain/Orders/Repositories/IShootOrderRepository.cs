using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Shoots.Entities;
using Reamp.Domain.Shoots.Enums;

namespace Reamp.Domain.Shoots.Repositories
{
    public interface IShootOrderRepository : IRepository<ShootOrder>
    {
        // Get order with all tasks included
        Task<ShootOrder?> GetAggregateAsync(Guid id, bool asNoTracking = false, CancellationToken ct = default);

        // List orders with pagination
        Task<IPagedList<ShootOrder>> ListAsync(
            PageRequest page,
            Guid? agencyId = null,
            Guid? studioId = null,
            Guid? createdBy = null,
            CancellationToken ct = default);

        // Advanced filtered list
        Task<IPagedList<ShootOrder>> ListFilteredAsync(
            PageRequest page,
            Guid? agencyId = null,
            Guid? studioId = null,
            Guid? listingId = null,
            Guid? photographerId = null,
            ShootOrderStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            Guid? createdBy = null,
            CancellationToken ct = default);

        // Update order
        Task UpdateAsync(ShootOrder entity, CancellationToken ct = default);
    }
}



