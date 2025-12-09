using Reamp.Domain.Media.Enums;

namespace Reamp.Application.Media.Dtos
{
    // DTO for media asset list item
    public class MediaAssetListDto
    {
        public Guid Id { get; set; }
        public Guid OwnerStudioId { get; set; }
        public string? StudioName { get; set; }
        public MediaResourceType ResourceType { get; set; }
        public MediaProcessStatus ProcessStatus { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public double? DurationSeconds { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public List<string>? Tags { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}



