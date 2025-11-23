using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool IsVisible { get; private set; }

        private ListingMediaRef() { }

        public ListingMediaRef(Guid listingId, Guid mediaAssetId, ListingMediaRole role, int sortOrder = 0, bool isCover = false, bool isVisible = true)
        {
            if (listingId == Guid.Empty) throw new ArgumentException(nameof(listingId));
            if (mediaAssetId == Guid.Empty) throw new ArgumentException(nameof(mediaAssetId));
            if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));

            Id = Guid.NewGuid();
            ListingId = listingId;
            MediaAssetId = mediaAssetId;
            Role = role;
            SortOrder = sortOrder;
            IsCover = isCover;
            IsVisible = isVisible;
        }

        public void UpdateRole(ListingMediaRole role) => Role = role;

        public void UpdateSortOrder(int sortOrder)
        {
            if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));
            SortOrder = sortOrder;
        }

        public void SetCover() => IsCover = true;
        public void UnsetCover() => IsCover = false;

        public void SetVisible(bool isVisible) => IsVisible = isVisible;

        public static ListingMediaRef CreateCover(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.Cover, sortOrder, true, true);

        public static ListingMediaRef CreateGallery(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.Gallery, sortOrder, false, true);

        public static ListingMediaRef CreateFloorPlan(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.FloorPlan, sortOrder, false, true);

        public static ListingMediaRef CreateVideo(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.TourVideo, sortOrder, false, true);

        public static ListingMediaRef CreateVrPreview(Guid listingId, Guid mediaAssetId, int sortOrder = 0)
            => new(listingId, mediaAssetId, ListingMediaRole.VRPreview, sortOrder, false, true);
    }
}