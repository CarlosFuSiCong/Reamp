using Microsoft.Extensions.Logging;
using Reamp.Application.Media.Dtos;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Media.Entities;
using Reamp.Domain.Media.Enums;
using Reamp.Domain.Media.Repositories;
using Reamp.Infrastructure.Configuration;
using Reamp.Infrastructure.Services.Media;

namespace Reamp.Application.Media.Services
{
    public sealed class MediaAssetAppService : IMediaAssetAppService
    {
        private readonly IMediaAssetRepository _mediaAssetRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MediaAssetAppService> _logger;
        private readonly MediaUploadSettings _uploadSettings;

        public MediaAssetAppService(
            IMediaAssetRepository mediaAssetRepository,
            ICloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork,
            Microsoft.Extensions.Options.IOptions<MediaUploadSettings> uploadSettings,
            ILogger<MediaAssetAppService> logger)
        {
            _mediaAssetRepository = mediaAssetRepository;
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
            _uploadSettings = uploadSettings.Value;
            _logger = logger;
        }

        public async Task<MediaAssetDetailDto> UploadAsync(UploadMediaDto dto, Guid currentUserId, CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            // Validate content type
            if (string.IsNullOrWhiteSpace(dto.ContentType))
                throw new ArgumentException("ContentType is required.", nameof(dto.ContentType));

            // Validate file
            var contentType = dto.ContentType.ToLowerInvariant();
            var isImage = contentType.StartsWith("image/");
            var isVideo = contentType.StartsWith("video/");

            if (!isImage && !isVideo)
                throw new InvalidOperationException("Only image and video files are supported.");

            // Validate file size
            var maxSize = isImage ? _uploadSettings.MaxImageSizeBytes : _uploadSettings.MaxVideoSizeBytes;
            if (dto.FileSize > maxSize)
                throw new InvalidOperationException($"File size exceeds maximum allowed ({maxSize} bytes).");

            // Validate file type
            var allowedTypes = isImage ? _uploadSettings.AllowedImageTypes : _uploadSettings.AllowedVideoTypes;
            if (allowedTypes != null && !allowedTypes.Contains(contentType))
                throw new InvalidOperationException($"File type '{contentType}' is not allowed.");

            // Compute checksum for deduplication (scoped to current studio)
            var checksum = await CloudinaryService.ComputeSha256Async(dto.FileStream);

            // Check for duplicate within the SAME studio only (security: prevent cross-tenant data leakage)
            var existing = await _mediaAssetRepository.FindByChecksumAsync(checksum, dto.OwnerStudioId, ct);
            if (existing != null)
            {
                _logger.LogInformation("Found duplicate media asset within studio: {AssetId}", existing.Id);
                return MapToDetailDto(existing);
            }

            // Upload to Cloudinary
            CloudinaryUploadResult uploadResult;
            uploadResult = isImage
                ? await _cloudinaryService.UploadImageAsync(dto.FileStream, dto.FileName, null, ct)
                : await _cloudinaryService.UploadVideoAsync(dto.FileStream, dto.FileName, null, ct);

            if (!uploadResult.IsSuccess)
            {
                _logger.LogError("Upload failed: {Error}", uploadResult.Error);
                throw new InvalidOperationException($"Upload failed: {uploadResult.Error}");
            }

            // Create media asset
            var mediaAsset = new MediaAsset(
                ownerStudioId: dto.OwnerStudioId,
                uploaderUserId: currentUserId,
                provider: MediaProvider.Cloudinary,
                providerAssetId: uploadResult.PublicId,
                resourceType: uploadResult.ResourceType,
                contentType: contentType,
                sizeBytes: uploadResult.Bytes,
                originalFileName: dto.FileName,
                publicUrl: uploadResult.SecureUrl,
                checksumSha256: checksum);

            // Update metadata
            mediaAsset.UpdateBasicMeta(
                uploadResult.Width,
                uploadResult.Height,
                uploadResult.Duration.HasValue ? (decimal?)uploadResult.Duration.Value : null);

            // Mark as ready
            mediaAsset.MarkReady();

            // Auto-generate thumbnail for video
            if (isVideo && _uploadSettings.GenerateThumbnails)
            {
                var thumbnailUrl = _cloudinaryService.GenerateVideoThumbnailUrl(
                    uploadResult.PublicId,
                    _uploadSettings.ThumbnailWidth,
                    _uploadSettings.ThumbnailHeight);

                mediaAsset.AddOrReplaceVariant(
                    name: "thumbnail",
                    url: thumbnailUrl,
                    format: "jpg",
                    width: _uploadSettings.ThumbnailWidth,
                    height: _uploadSettings.ThumbnailHeight,
                    sortOrder: 0);
            }

            await _mediaAssetRepository.AddAsync(mediaAsset, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Media asset uploaded: {AssetId}", mediaAsset.Id);

            return MapToDetailDto(mediaAsset);
        }

        public async Task<MediaAssetDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var asset = await _mediaAssetRepository.GetByIdAsync(id, asNoTracking: true, ct);
            return asset == null ? null : MapToDetailDto(asset);
        }

        public async Task<IPagedList<MediaAssetListDto>> ListByStudioAsync(
            Guid studioId,
            MediaResourceType? resourceType = null,
            MediaProcessStatus? status = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            var pagedAssets = await _mediaAssetRepository.ListByStudioAsync(
                studioId, resourceType, status, page, pageSize, ct);

            var dtos = pagedAssets.Items.Select(MapToListDto).ToList();

            return new PagedList<MediaAssetListDto>(
                dtos,
                pagedAssets.TotalCount,
                pagedAssets.Page,
                pagedAssets.PageSize);
        }

        public async Task<MediaAssetDetailDto> AddVariantAsync(Guid assetId, AddVariantDto dto, CancellationToken ct = default)
        {
            var asset = await _mediaAssetRepository.GetByIdAsync(assetId, asNoTracking: false, ct);
            if (asset == null)
                throw new KeyNotFoundException($"Media asset with ID {assetId} not found.");

            // Generate transformation URL
            var transformedUrl = _cloudinaryService.GenerateTransformationUrl(
                asset.ProviderAssetId,
                dto.Width,
                dto.Height,
                asset.ResourceType);

            asset.AddOrReplaceVariant(
                name: dto.VariantName,
                url: transformedUrl,
                width: dto.Width,
                height: dto.Height);

            await _unitOfWork.SaveChangesAsync(ct);

            return MapToDetailDto(asset);
        }

        public async Task TriggerProcessingAsync(Guid assetId, CancellationToken ct = default)
        {
            var asset = await _mediaAssetRepository.GetByIdAsync(assetId, asNoTracking: false, ct);
            if (asset == null)
                throw new KeyNotFoundException($"Media asset with ID {assetId} not found.");

            asset.StartProcessing();
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Processing triggered for asset: {AssetId}", assetId);

            // Queue background job using Hangfire (if available)
            // Note: Hangfire BackgroundJob.Enqueue requires Hangfire to be configured
            // For now, we'll just mark it as processing
            // In production with Hangfire: BackgroundJob.Enqueue<IMediaProcessingJob>(x => x.OptimizeMediaAsync(assetId));
        }

        public async Task DeleteAsync(Guid assetId, CancellationToken ct = default)
        {
            var asset = await _mediaAssetRepository.GetByIdAsync(assetId, asNoTracking: false, ct);
            if (asset == null)
                throw new KeyNotFoundException($"Media asset with ID {assetId} not found.");

            // Delete from Cloudinary
            var deleted = await _cloudinaryService.DeleteAssetAsync(
                asset.ProviderAssetId,
                asset.ResourceType,
                ct);

            if (!deleted)
            {
                _logger.LogWarning("Failed to delete asset from Cloudinary: {AssetId}", assetId);
            }

            _mediaAssetRepository.Remove(asset);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Media asset deleted: {AssetId}", assetId);
        }

        private MediaAssetDetailDto MapToDetailDto(MediaAsset asset)
        {
            return new MediaAssetDetailDto
            {
                Id = asset.Id,
                OwnerStudioId = asset.OwnerStudioId,
                UploaderUserId = asset.UploaderUserId,
                MediaProvider = asset.Provider,
                ProviderAssetId = asset.ProviderAssetId,
                ResourceType = asset.ResourceType,
                ProcessStatus = asset.ProcessStatus,
                ContentType = asset.ContentType,
                SizeBytes = asset.SizeBytes,
                WidthPx = asset.WidthPx,
                HeightPx = asset.HeightPx,
                DurationSeconds = asset.DurationSeconds.HasValue ? (double?)asset.DurationSeconds.Value : null,
                OriginalFileName = asset.OriginalFileName ?? string.Empty,
                PublicUrl = asset.PublicUrl ?? string.Empty,
                ChecksumSha256 = asset.ChecksumSha256,
                Variants = asset.Variants.Select(v => new MediaVariantDto
                {
                    VariantName = v.Name,
                    TransformedUrl = v.Url,
                    WidthPx = v.WidthPx,
                    HeightPx = v.HeightPx,
                    SizeBytes = v.SizeBytes
                }).ToList(),
                CreatedAtUtc = asset.CreatedAtUtc,
                UpdatedAtUtc = asset.UpdatedAtUtc
            };
        }

        private MediaAssetListDto MapToListDto(MediaAsset asset)
        {
            string? thumbnailUrl = null;
            if (asset.ResourceType == MediaResourceType.Video && asset.Variants.Any())
            {
                thumbnailUrl = asset.Variants
                    .FirstOrDefault(v => v.Name == "thumbnail")?.Url;
            }

            return new MediaAssetListDto
            {
                Id = asset.Id,
                OwnerStudioId = asset.OwnerStudioId,
                ResourceType = asset.ResourceType,
                ProcessStatus = asset.ProcessStatus,
                ContentType = asset.ContentType,
                SizeBytes = asset.SizeBytes,
                WidthPx = asset.WidthPx,
                HeightPx = asset.HeightPx,
                DurationSeconds = asset.DurationSeconds.HasValue ? (double?)asset.DurationSeconds.Value : null,
                OriginalFileName = asset.OriginalFileName ?? string.Empty,
                PublicUrl = asset.PublicUrl ?? string.Empty,
                ThumbnailUrl = thumbnailUrl,
                CreatedAtUtc = asset.CreatedAtUtc
            };
        }
    }
}

