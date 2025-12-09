using Reamp.Domain.Media.Enums;

namespace Reamp.Infrastructure.Services.Media
{
    // Cloudinary media upload and management service
    public interface ICloudinaryService
    {
        // Test connection to Cloudinary
        Task<bool> TestConnectionAsync();

        // Get cloud information
        Task<string> GetCloudInfoAsync();

        // Upload image to Cloudinary
        Task<CloudinaryUploadResult> UploadImageAsync(
            Stream stream,
            string fileName,
            string? folder = null,
            CancellationToken ct = default);

        // Upload video to Cloudinary
        Task<CloudinaryUploadResult> UploadVideoAsync(
            Stream stream,
            string fileName,
            string? folder = null,
            CancellationToken ct = default);

        // Delete asset from Cloudinary
        Task<bool> DeleteAssetAsync(
            string publicId,
            MediaResourceType resourceType,
            CancellationToken ct = default);

        // Generate transformation URL for an asset
        string GenerateTransformationUrl(
            string publicId,
            int? width,
            int? height,
            MediaResourceType resourceType);

        // Generate thumbnail URL for video
        string GenerateVideoThumbnailUrl(
            string publicId,
            int width = 640,
            int height = 360);
    }

    // Result of Cloudinary upload operation
    public class CloudinaryUploadResult
    {
        public string PublicId { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public long Bytes { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public double? Duration { get; set; }
        public MediaResourceType ResourceType { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(Error);
    }
}

