using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Delivery.Entities;
using Reamp.Domain.Delivery.Enums;
using Reamp.Domain.Delivery.Repositories;
using Reamp.Infrastructure.Repositories.Common;

namespace Reamp.Infrastructure.Repositories.Delivery
{
    public sealed class DeliveryPackageRepository : BaseRepository<DeliveryPackage>, IDeliveryPackageRepository
    {
        public DeliveryPackageRepository(ApplicationDbContext db, ILogger<DeliveryPackageRepository> logger)
            : base(db, logger)
        {
        }

        public async Task<DeliveryPackage?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _set
                .Include(d => d.Items)
                    .ThenInclude(i => i.MediaAsset)
                .Include(d => d.Accesses)
                .FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<List<DeliveryPackage>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            return await _set
                .Include(d => d.Items)
                .Include(d => d.Accesses)
                .Where(d => d.OrderId == orderId)
                .OrderByDescending(d => d.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<List<DeliveryPackage>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            return await _set
                .Include(d => d.Items)
                .Include(d => d.Accesses)
                .Where(d => d.ListingId == listingId)
                .OrderByDescending(d => d.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<List<DeliveryPackage>> GetByStatusAsync(DeliveryStatus status, int limit = 100, CancellationToken ct = default)
        {
            return await _set
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.CreatedAtUtc)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task<List<DeliveryPackage>> GetExpiredPackagesAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _set
                .Where(d => d.Status == DeliveryStatus.Published &&
                           d.ExpiresAtUtc.HasValue &&
                           d.ExpiresAtUtc.Value <= now)
                .ToListAsync(ct);
        }

        public async Task AddItemsDirectlyAsync(Guid packageId, List<DeliveryItem> items, CancellationToken ct = default)
        {
            // Add items directly to DbSet without loading parent package
            // This avoids triggering parent entity updates and RowVersion conflicts
            await _db.Set<DeliveryItem>().AddRangeAsync(items, ct);
        }
    }
}

