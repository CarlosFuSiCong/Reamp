namespace Reamp.Infrastructure.Configuration
{
    public sealed class MediaUploadSettings
    {
        public long MaxImageSizeBytes { get; set; } = 10485760; // 10MB
        public long MaxVideoSizeBytes { get; set; } = 104857600; // 100MB
        public List<string> AllowedImageTypes { get; set; } = new();
        public List<string> AllowedVideoTypes { get; set; } = new();
        public bool EnableAutoOptimization { get; set; } = true;
        public string VideoQuality { get; set; } = "auto";
        public bool GenerateThumbnails { get; set; } = true;
        public int ThumbnailWidth { get; set; } = 640;
        public int ThumbnailHeight { get; set; } = 360;
        public int ChunkSizeBytes { get; set; } = 5242880; // 5MB
    }
}



