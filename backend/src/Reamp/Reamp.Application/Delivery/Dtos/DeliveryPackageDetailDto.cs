using Reamp.Domain.Delivery.Enums;

namespace Reamp.Application.Delivery.Dtos
{
    public sealed class DeliveryPackageDetailDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ListingId { get; set; }
        public string Title { get; set; } = default!;
        public DeliveryStatus Status { get; set; }
        public bool WatermarkEnabled { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        
        public List<DeliveryItemDto> Items { get; set; } = new();
        public List<DeliveryAccessDto> Accesses { get; set; } = new();
        
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }

    public sealed class DeliveryItemDto
    {
        public Guid Id { get; set; }
        public Guid MediaAssetId { get; set; }
        public string VariantName { get; set; } = default!;
        public int SortOrder { get; set; }
        public string? MediaUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    public sealed class DeliveryAccessDto
    {
        public Guid Id { get; set; }
        public AccessType Type { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientName { get; set; }
        public int? MaxDownloads { get; set; }
        public int Downloads { get; set; }
        public bool HasPassword { get; set; }
    }
}

