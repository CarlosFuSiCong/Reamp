using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Delivery.Entities
{
    public sealed class DeliveryItem
    {
        public Guid Id { get; private set; }
        public Guid DeliveryPackageId { get; private set; }
        public Guid MediaAssetId { get; private set; }
        public string VariantName { get; private set; } = default!; // e.g. wm_1920 / web_1920 / hls
        public int SortOrder { get; private set; }

        private DeliveryItem() { }

        public static DeliveryItem Create(Guid packageId, Guid mediaAssetId, string variantName, int sortOrder)
        {
            if (packageId == Guid.Empty) throw new ArgumentException("PackageId required");
            if (mediaAssetId == Guid.Empty) throw new ArgumentException("MediaAssetId required");
            if (string.IsNullOrWhiteSpace(variantName)) throw new ArgumentException("VariantName required");

            return new DeliveryItem
            {
                Id = Guid.NewGuid(),
                DeliveryPackageId = packageId,
                MediaAssetId = mediaAssetId,
                VariantName = variantName.Trim(),
                SortOrder = sortOrder
            };
        }
    }
}
