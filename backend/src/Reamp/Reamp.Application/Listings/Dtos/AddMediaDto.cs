using Reamp.Domain.Listings.Enums;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class AddMediaDto
    {
        public Guid MediaAssetId { get; set; }
        public ListingMediaRole Role { get; set; } = ListingMediaRole.Gallery;
        public int SortOrder { get; set; } = 0;
    }
}

