using Mapster;
using Microsoft.Extensions.Logging;
using Reamp.Application.Delivery.Dtos;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Delivery.Entities;
using Reamp.Domain.Delivery.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Reamp.Application.Delivery.Services
{
    public sealed class DeliveryPackageAppService : IDeliveryPackageAppService
    {
        private readonly IDeliveryPackageRepository _deliveryRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeliveryPackageAppService> _logger;

        public DeliveryPackageAppService(
            IDeliveryPackageRepository deliveryRepo,
            IUnitOfWork uow,
            ILogger<DeliveryPackageAppService> logger)
        {
            _deliveryRepo = deliveryRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<DeliveryPackageDetailDto> CreateAsync(CreateDeliveryPackageDto dto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating delivery package for OrderId: {OrderId}", dto.OrderId);

            var package = DeliveryPackage.Create(
                orderId: dto.OrderId,
                listingId: dto.ListingId,
                title: dto.Title,
                watermarkEnabled: dto.WatermarkEnabled,
                expiresAtUtc: dto.ExpiresAtUtc
            );

            await _deliveryRepo.AddAsync(package, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package created with ID: {PackageId}", package.Id);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<DeliveryPackageDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            return package?.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<List<DeliveryPackageListDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            var packages = await _deliveryRepo.GetByOrderIdAsync(orderId, ct);
            return packages.Adapt<List<DeliveryPackageListDto>>();
        }

        public async Task<List<DeliveryPackageListDto>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            var packages = await _deliveryRepo.GetByListingIdAsync(listingId, ct);
            return packages.Adapt<List<DeliveryPackageListDto>>();
        }

        public async Task<DeliveryPackageDetailDto> UpdateAsync(Guid id, UpdateDeliveryPackageDto dto, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            package.UpdateDetails(dto.Title, dto.WatermarkEnabled, dto.ExpiresAtUtc);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} updated", id);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdAsync(id, false, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            package.SoftDelete();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} deleted", id);
        }

        public async Task<DeliveryPackageDetailDto> AddItemAsync(Guid packageId, AddDeliveryItemDto dto, CancellationToken ct = default)
        {
            // Use batch method for consistency
            return await AddItemsBatchAsync(packageId, new List<AddDeliveryItemDto> { dto }, ct);
        }

        public async Task<DeliveryPackageDetailDto> AddItemsBatchAsync(Guid packageId, List<AddDeliveryItemDto> items, CancellationToken ct = default)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("At least one item is required", nameof(items));

            // Verify package exists (use no tracking to avoid RowVersion conflicts)
            var packageExists = await _deliveryRepo.GetByIdAsync(packageId, asNoTracking: true, ct);
            if (packageExists == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            _logger.LogInformation("Adding {Count} item(s) to delivery package {PackageId}", items.Count, packageId);

            // Create DeliveryItem entities
            var deliveryItems = items.Select(dto => 
                DeliveryItem.Create(packageId, dto.MediaAssetId, dto.VariantName, dto.SortOrder)
            ).ToList();

            // Add items directly to database without loading parent package
            // This avoids triggering parent entity updates and RowVersion conflicts
            await _deliveryRepo.AddItemsDirectlyAsync(packageId, deliveryItems, ct);
            
            // Save once after all items are added
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully added {Count} item(s) to delivery package {PackageId}", items.Count, packageId);
            
            // Reload package with items to return
            var updatedPackage = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            return updatedPackage!.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<DeliveryPackageDetailDto> RemoveItemAsync(Guid packageId, Guid itemId, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            package.RemoveItem(itemId);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Item {ItemId} removed from delivery package {PackageId}", itemId, packageId);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<DeliveryPackageDetailDto> AddAccessAsync(Guid packageId, AddDeliveryAccessDto dto, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            string? passwordHash = null;
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                passwordHash = HashPassword(dto.Password);
            }

            package.AddAccess(
                type: dto.Type,
                recipientEmail: dto.RecipientEmail,
                recipientName: dto.RecipientName,
                maxDownloads: dto.MaxDownloads,
                passwordHash: passwordHash
            );

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Access added to delivery package {PackageId}", packageId);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<DeliveryPackageDetailDto> RemoveAccessAsync(Guid packageId, Guid accessId, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            package.RemoveAccess(accessId);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Access {AccessId} removed from delivery package {PackageId}", accessId, packageId);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<DeliveryPackageDetailDto> PublishAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            // Publish the delivery
            package.Publish();

            // TODO: Update related order status to AwaitingConfirmation
            // This will be implemented when Order domain methods are added
            // For now, the status update will be handled by the Order service

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} published for order {OrderId}", id, package.OrderId);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<DeliveryPackageDetailDto> RevokeAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            package.Revoke();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} revoked", id);
            return package.Adapt<DeliveryPackageDetailDto>();
        }

        public async Task<bool> VerifyAccessPasswordAsync(Guid packageId, string password, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                return false;

            var access = package.Accesses.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PasswordHash));
            if (access == null)
                return true; // No password protection

            var passwordHash = HashPassword(password);
            return access.PasswordHash == passwordHash;
        }

        public async Task IncrementDownloadAsync(Guid packageId, Guid accessId, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            var access = package.Accesses.FirstOrDefault(a => a.Id == accessId);
            if (access == null)
                throw new KeyNotFoundException($"Access with ID {accessId} not found in package {packageId}");

            access.IncrementDownloads();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Download count incremented for access {AccessId} in package {PackageId}",
                accessId, packageId);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

