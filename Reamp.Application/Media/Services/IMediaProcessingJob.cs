namespace Reamp.Application.Media.Services
{
    // Background job interface for media processing
    public interface IMediaProcessingJob
    {
        // Generate thumbnail for video (executed by Hangfire)
        Task GenerateVideoThumbnailAsync(Guid mediaAssetId);

        // Process media optimization (executed by Hangfire)
        Task OptimizeMediaAsync(Guid mediaAssetId);

        // Sync media status from Cloudinary (executed by Hangfire)
        Task SyncMediaStatusAsync(Guid mediaAssetId);
    }
}

