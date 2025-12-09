using Reamp.Domain.Media.Enums;

namespace Reamp.Application.Media.Dtos
{
    // DTO for media asset detail
    public class MediaAssetDetailDto
    {
        public Guid Id { get; set; }
        public Guid OwnerStudioId { get; set; }
        public string? StudioName { get; set; }
        public Guid UploaderUserId { get; set; }
        public string? UploaderName { get; set; }
        public MediaProvider MediaProvider { get; set; }
        public string ProviderAssetId { get; set; } = string.Empty;
        public MediaResourceType ResourceType { get; set; }
        public MediaProcessStatus ProcessStatus { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public double? DurationSeconds { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
        public string? ChecksumSha256 { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
        public List<MediaVariantDto> Variants { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }

    // DTO for media variant
    public class MediaVariantDto
    {
        public string VariantName { get; set; } = string.Empty;
        public string TransformedUrl { get; set; } = string.Empty;
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public long? SizeBytes { get; set; }
    }
}



