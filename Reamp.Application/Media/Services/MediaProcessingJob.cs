using Microsoft.Extensions.Logging;
using Reamp.Domain.Media.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Services.Media;

namespace Reamp.Application.Media.Services
{
    public sealed class MediaProcessingJob : IMediaProcessingJob
    {
        private readonly IMediaAssetRepository _mediaAssetRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MediaProcessingJob> _logger;

        public MediaProcessingJob(
            IMediaAssetRepository mediaAssetRepository,
            ICloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork,
            ILogger<MediaProcessingJob> logger)
        {
            _mediaAssetRepository = mediaAssetRepository;
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task GenerateVideoThumbnailAsync(Guid mediaAssetId)
        {
            _logger.LogInformation("Starting thumbnail generation for media asset {AssetId}", mediaAssetId);

            try
            {
                var asset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, asNoTracking: false);
                if (asset == null)
                {
                    _logger.LogWarning("Media asset {AssetId} not found", mediaAssetId);
                    return;
                }

                if (asset.ResourceType != Domain.Media.Enums.MediaResourceType.Video)
                {
                    _logger.LogWarning("Media asset {AssetId} is not a video", mediaAssetId);
                    return;
                }

                // Generate thumbnail URL
                var thumbnailUrl = _cloudinaryService.GenerateVideoThumbnailUrl(
                    asset.ProviderAssetId,
                    640,
                    360);

                // Add thumbnail variant
                asset.AddOrReplaceVariant(
                    name: "thumbnail",
                    url: thumbnailUrl,
                    format: "jpg",
                    width: 640,
                    height: 360,
                    sortOrder: 0);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Thumbnail generated successfully for media asset {AssetId}", mediaAssetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating thumbnail for media asset {AssetId}", mediaAssetId);
                throw;
            }
        }

        public async Task OptimizeMediaAsync(Guid mediaAssetId)
        {
            _logger.LogInformation("Starting media optimization for asset {AssetId}", mediaAssetId);

            try
            {
                var asset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, asNoTracking: false);
                if (asset == null)
                {
                    _logger.LogWarning("Media asset {AssetId} not found", mediaAssetId);
                    return;
                }

                asset.StartProcessing();
                await _unitOfWork.SaveChangesAsync();

                // Simulate optimization process
                await Task.Delay(2000);

                // Generate optimized variants
                var sizes = new[] { (320, 240), (640, 480), (1280, 720), (1920, 1080) };
                foreach (var (width, height) in sizes)
                {
                    var variantUrl = _cloudinaryService.GenerateTransformationUrl(
                        asset.ProviderAssetId,
                        width,
                        height,
                        asset.ResourceType);

                    asset.AddOrReplaceVariant(
                        name: $"{width}x{height}",
                        url: variantUrl,
                        width: width,
                        height: height,
                        sortOrder: 0);
                }

                asset.MarkReady();
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Media optimization completed for asset {AssetId}", mediaAssetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing media asset {AssetId}", mediaAssetId);

                var asset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, asNoTracking: false);
                if (asset != null)
                {
                    asset.MarkFailed();
                    await _unitOfWork.SaveChangesAsync();
                }

                throw;
            }
        }

        public async Task SyncMediaStatusAsync(Guid mediaAssetId)
        {
            _logger.LogInformation("Syncing media status for asset {AssetId}", mediaAssetId);

            try
            {
                var asset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId, asNoTracking: false);
                if (asset == null)
                {
                    _logger.LogWarning("Media asset {AssetId} not found", mediaAssetId);
                    return;
                }

                // In a real implementation, you would query Cloudinary API for the asset status
                // For now, just mark as ready
                if (asset.ProcessStatus == Domain.Media.Enums.MediaProcessStatus.Processing)
                {
                    asset.MarkReady();
                    await _unitOfWork.SaveChangesAsync();
                }

                _logger.LogInformation("Media status synced for asset {AssetId}", mediaAssetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing media status for asset {AssetId}", mediaAssetId);
                throw;
            }
        }
    }
}

