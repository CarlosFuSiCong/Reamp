using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Delivery.Entities;
using Reamp.Domain.Delivery.Enums;

namespace Reamp.Domain.Delivery.Repositories
{
    public interface IDeliveryPackageRepository : IRepository<DeliveryPackage>
    {
        // Get delivery package by ID with items and accesses
        Task<DeliveryPackage?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

        // Get delivery packages by OrderId
        Task<List<DeliveryPackage>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);

        // Get delivery packages by ListingId
        Task<List<DeliveryPackage>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default);

        // Get delivery packages by status
        Task<List<DeliveryPackage>> GetByStatusAsync(DeliveryStatus status, int limit = 100, CancellationToken ct = default);

        // Get expired packages that need cleanup
        Task<List<DeliveryPackage>> GetExpiredPackagesAsync(CancellationToken ct = default);
        
        // Add delivery items without loading parent package (avoids RowVersion conflicts)
        Task AddItemsDirectlyAsync(Guid packageId, List<DeliveryItem> items, CancellationToken ct = default);
    }
}

