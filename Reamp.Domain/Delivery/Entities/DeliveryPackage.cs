using Reamp.Domain.Common.Entities;
using Reamp.Domain.Delivery.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Delivery.Entities
{
    public sealed class DeliveryPackage : AuditableEntity
    {
        public Guid OrderId { get; private set; }           // ShootOrder Id
        public Guid ListingId { get; private set; }         // linked listing
        public string Title { get; private set; } = default!;
        public DeliveryStatus Status { get; private set; } = DeliveryStatus.Draft;

        public bool WatermarkEnabled { get; private set; }
        public DateTime? ExpiresAtUtc { get; private set; }

        private readonly List<DeliveryItem> _items = new();
        public IReadOnlyCollection<DeliveryItem> Items => _items.AsReadOnly();

        private readonly List<DeliveryAccess> _accesses = new();
        public IReadOnlyCollection<DeliveryAccess> Accesses => _accesses.AsReadOnly();

        private DeliveryPackage() { }

        public static DeliveryPackage Create(Guid orderId, Guid listingId, string title, bool watermarkEnabled = false, DateTime? expiresAtUtc = null)
        {
            if (orderId == Guid.Empty) throw new ArgumentException("OrderId required");
            if (listingId == Guid.Empty) throw new ArgumentException("ListingId required");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title required");

            return new DeliveryPackage
            {
                OrderId = orderId,
                ListingId = listingId,
                Title = title.Trim(),
                WatermarkEnabled = watermarkEnabled,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        public void Publish()
        {
            if (Status != DeliveryStatus.Draft) throw new InvalidOperationException("Must be draft");
            if (_items.Count == 0) throw new InvalidOperationException("No items to publish");
            Status = DeliveryStatus.Published; Touch();
        }

        public void Revoke()
        {
            if (Status != DeliveryStatus.Published) throw new InvalidOperationException("Must be published");
            Status = DeliveryStatus.Revoked; Touch();
        }

        public void ExpireNow()
        {
            ExpiresAtUtc = DateTime.UtcNow;
            Status = DeliveryStatus.Expired; Touch();
        }

        public DeliveryItem AddItem(Guid mediaAssetId, string variantName, int sortOrder = 0)
        {
            if (mediaAssetId == Guid.Empty) throw new ArgumentException("MediaAssetId required");
            if (string.IsNullOrWhiteSpace(variantName)) throw new ArgumentException("VariantName required");

            var item = DeliveryItem.Create(Id == Guid.Empty ? Guid.NewGuid() : Id, mediaAssetId, variantName, sortOrder);
            _items.Add(item);
            Touch();
            return item;
        }

        public DeliveryAccess AddAccess(AccessType type, string? recipientEmail = null, string? recipientName = null, int? maxDownloads = null, string? passwordHash = null)
        {
            var acc = DeliveryAccess.Create(Id == Guid.Empty ? Guid.NewGuid() : Id, type, recipientEmail, recipientName, maxDownloads, passwordHash);
            _accesses.Add(acc);
            Touch();
            return acc;
        }
    }
}