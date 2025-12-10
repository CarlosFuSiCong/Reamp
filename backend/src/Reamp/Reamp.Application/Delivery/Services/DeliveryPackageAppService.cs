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
            return MapToDetailDto(package);
        }

        public async Task<DeliveryPackageDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            return package == null ? null : MapToDetailDto(package);
        }

        public async Task<List<DeliveryPackageListDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            var packages = await _deliveryRepo.GetByOrderIdAsync(orderId, ct);
            return packages.Select(MapToListDto).ToList();
        }

        public async Task<List<DeliveryPackageListDto>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            var packages = await _deliveryRepo.GetByListingIdAsync(listingId, ct);
            return packages.Select(MapToListDto).ToList();
        }

        public async Task<DeliveryPackageDetailDto> UpdateAsync(Guid id, UpdateDeliveryPackageDto dto, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                // Use reflection to update private setter
                var titleProperty = typeof(DeliveryPackage).GetProperty(nameof(DeliveryPackage.Title));
                titleProperty?.SetValue(package, dto.Title.Trim());
            }

            if (dto.WatermarkEnabled.HasValue)
            {
                var watermarkProperty = typeof(DeliveryPackage).GetProperty(nameof(DeliveryPackage.WatermarkEnabled));
                watermarkProperty?.SetValue(package, dto.WatermarkEnabled.Value);
            }

            if (dto.ExpiresAtUtc.HasValue)
            {
                var expiresProperty = typeof(DeliveryPackage).GetProperty(nameof(DeliveryPackage.ExpiresAtUtc));
                expiresProperty?.SetValue(package, dto.ExpiresAtUtc.Value);
            }

            package.MarkUpdated();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} updated", id);
            return MapToDetailDto(package);
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
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            package.AddItem(dto.MediaAssetId, dto.VariantName, dto.SortOrder);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Item added to delivery package {PackageId}", packageId);
            return MapToDetailDto(package);
        }

        public async Task<DeliveryPackageDetailDto> RemoveItemAsync(Guid packageId, Guid itemId, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            // Use reflection to remove item from private collection
            var itemsField = typeof(DeliveryPackage).GetField("_items", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (itemsField?.GetValue(package) is List<DeliveryItem> items)
            {
                var itemToRemove = items.FirstOrDefault(i => i.Id == itemId);
                if (itemToRemove != null)
                {
                    items.Remove(itemToRemove);
                    package.MarkUpdated();
                }
            }

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Item {ItemId} removed from delivery package {PackageId}", itemId, packageId);
            return MapToDetailDto(package);
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
            return MapToDetailDto(package);
        }

        public async Task<DeliveryPackageDetailDto> RemoveAccessAsync(Guid packageId, Guid accessId, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(packageId, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {packageId} not found");

            // Use reflection to remove access from private collection
            var accessesField = typeof(DeliveryPackage).GetField("_accesses",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (accessesField?.GetValue(package) is List<DeliveryAccess> accesses)
            {
                var accessToRemove = accesses.FirstOrDefault(a => a.Id == accessId);
                if (accessToRemove != null)
                {
                    accesses.Remove(accessToRemove);
                    package.MarkUpdated();
                }
            }

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Access {AccessId} removed from delivery package {PackageId}", accessId, packageId);
            return MapToDetailDto(package);
        }

        public async Task<DeliveryPackageDetailDto> PublishAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            package.Publish();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} published", id);
            return MapToDetailDto(package);
        }

        public async Task<DeliveryPackageDetailDto> RevokeAsync(Guid id, CancellationToken ct = default)
        {
            var package = await _deliveryRepo.GetByIdWithDetailsAsync(id, ct);
            if (package == null)
                throw new KeyNotFoundException($"Delivery package with ID {id} not found");

            package.Revoke();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery package {PackageId} revoked", id);
            return MapToDetailDto(package);
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
            if (access != null)
            {
                access.IncrementDownloads();
                await _uow.SaveChangesAsync(ct);

                _logger.LogInformation("Download count incremented for access {AccessId} in package {PackageId}",
                    accessId, packageId);
            }
        }

        // Helper method to hash password
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Mapping methods
        private static DeliveryPackageDetailDto MapToDetailDto(DeliveryPackage package)
        {
            return new DeliveryPackageDetailDto
            {
                Id = package.Id,
                OrderId = package.OrderId,
                ListingId = package.ListingId,
                Title = package.Title,
                Status = package.Status,
                WatermarkEnabled = package.WatermarkEnabled,
                ExpiresAtUtc = package.ExpiresAtUtc,
                Items = package.Items.Select(i => new DeliveryItemDto
                {
                    Id = i.Id,
                    MediaAssetId = i.MediaAssetId,
                    VariantName = i.VariantName,
                    SortOrder = i.SortOrder
                }).ToList(),
                Accesses = package.Accesses.Select(a => new DeliveryAccessDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    RecipientEmail = a.RecipientEmail,
                    RecipientName = a.RecipientName,
                    MaxDownloads = a.MaxDownloads,
                    Downloads = a.Downloads,
                    HasPassword = !string.IsNullOrWhiteSpace(a.PasswordHash)
                }).ToList(),
                CreatedAtUtc = package.CreatedAtUtc,
                UpdatedAtUtc = package.UpdatedAtUtc
            };
        }

        private static DeliveryPackageListDto MapToListDto(DeliveryPackage package)
        {
            return new DeliveryPackageListDto
            {
                Id = package.Id,
                OrderId = package.OrderId,
                ListingId = package.ListingId,
                Title = package.Title,
                Status = package.Status,
                ItemCount = package.Items.Count,
                ExpiresAtUtc = package.ExpiresAtUtc,
                CreatedAtUtc = package.CreatedAtUtc
            };
        }
    }
}

