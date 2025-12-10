using Reamp.Application.Delivery.Dtos;

namespace Reamp.Application.Delivery.Services
{
    public interface IDeliveryPackageAppService
    {
        // Create a new delivery package
        Task<DeliveryPackageDetailDto> CreateAsync(CreateDeliveryPackageDto dto, CancellationToken ct = default);

        // Get delivery package by ID
        Task<DeliveryPackageDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

        // Get delivery packages by OrderId
        Task<List<DeliveryPackageListDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);

        // Get delivery packages by ListingId
        Task<List<DeliveryPackageListDto>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default);

        // Update delivery package details
        Task<DeliveryPackageDetailDto> UpdateAsync(Guid id, UpdateDeliveryPackageDto dto, CancellationToken ct = default);

        // Delete delivery package (soft delete)
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        // Add item to delivery package
        Task<DeliveryPackageDetailDto> AddItemAsync(Guid packageId, AddDeliveryItemDto dto, CancellationToken ct = default);

        // Remove item from delivery package
        Task<DeliveryPackageDetailDto> RemoveItemAsync(Guid packageId, Guid itemId, CancellationToken ct = default);

        // Add access control to delivery package
        Task<DeliveryPackageDetailDto> AddAccessAsync(Guid packageId, AddDeliveryAccessDto dto, CancellationToken ct = default);

        // Remove access from delivery package
        Task<DeliveryPackageDetailDto> RemoveAccessAsync(Guid packageId, Guid accessId, CancellationToken ct = default);

        // Publish delivery package
        Task<DeliveryPackageDetailDto> PublishAsync(Guid id, CancellationToken ct = default);

        // Revoke published delivery package
        Task<DeliveryPackageDetailDto> RevokeAsync(Guid id, CancellationToken ct = default);

        // Verify access password (for public access)
        Task<bool> VerifyAccessPasswordAsync(Guid packageId, string password, CancellationToken ct = default);

        // Increment download counter for an access
        Task IncrementDownloadAsync(Guid packageId, Guid accessId, CancellationToken ct = default);
    }
}

