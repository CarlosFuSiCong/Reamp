using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reamp.Domain.Listings.Enums;
using System;

namespace Reamp.Domain.Listings.Entities
{
    public sealed class ListingMediaRef
    {
        public Guid Id { get; private set; }
        public Guid ListingId { get; private set; }
        public Guid MediaAssetId { get; private set; }

        public ListingMediaRole Role { get; private set; }

        public int SortOrder { get; private set; }
        public bool IsCover { get; private set; }

        private ListingMediaRef() { } // EF

        public ListingMediaRef(Guid listingId, Guid mediaAssetId, ListingMediaRole role, int sortOrder = 0, bool isCover = false)
        {
            if (listingId == Guid.Empty) throw new ArgumentException("ListingId is required.", nameof(listingId));
            if (mediaAssetId == Guid.Empty) throw new ArgumentException("MediaAssetId is required.", nameof(mediaAssetId));
            if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder), "SortOrder cannot be negative.");

            Id = Guid.NewGuid();
            ListingId = listingId;
            MediaAssetId = mediaAssetId;
            Role = role;
            SortOrder = sortOrder;
            IsCover = isCover;
        }

        public void UpdateRole(ListingMediaRole role) => Role = role;

        public void UpdateSortOrder(int sortOrder)
        {
            if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));
            SortOrder = sortOrder;
        }

        public void SetCover() => IsCover = true;
        public void UnsetCover() => IsCover = false;

        public static ListingMediaRef CreateCover(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.Cover, sortOrder, true);

        public static ListingMediaRef CreateGallery(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.Gallery, sortOrder);

        public static ListingMediaRef CreateFloorPlan(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.FloorPlan, sortOrder);

        public static ListingMediaRef CreateVideo(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.TourVideo, sortOrder);

        public static ListingMediaRef CreateVrPreview(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.VRPreview, sortOrder);
    }
}
