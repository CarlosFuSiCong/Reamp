using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Shoots.Entities;

namespace Reamp.Domain.Shoots.Repositories
{
    public interface IShootOrderRepository : IRepository<ShootOrder>
    {
        // Get order with all tasks included
        Task<ShootOrder?> GetAggregateAsync(Guid id, CancellationToken ct = default);

        // List orders with pagination
        Task<IPagedList<ShootOrder>> ListAsync(
            PageRequest page,
            Guid? agencyId = null,
            Guid? studioId = null,
            Guid? createdBy = null,
            CancellationToken ct = default);

        // Update order
        Task UpdateAsync(ShootOrder entity, CancellationToken ct = default);
    }
}

