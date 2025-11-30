using Reamp.Application.Media.Dtos;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Media.Enums;

namespace Reamp.Application.Media.Services
{
    public interface IMediaAssetAppService
    {
        // Upload media (image or video)
        Task<MediaAssetDetailDto> UploadAsync(UploadMediaDto dto, Guid currentUserId, CancellationToken ct = default);

        // Get media asset by ID
        Task<MediaAssetDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

        // List media assets by studio
        Task<IPagedList<MediaAssetListDto>> ListByStudioAsync(
            Guid studioId,
            MediaResourceType? resourceType = null,
            MediaProcessStatus? status = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);

        // Add variant to media asset
        Task<MediaAssetDetailDto> AddVariantAsync(Guid assetId, AddVariantDto dto, CancellationToken ct = default);

        // Trigger processing for media asset
        Task TriggerProcessingAsync(Guid assetId, CancellationToken ct = default);

        // Delete media asset (with ownership verification)
        Task DeleteAsync(Guid assetId, Guid studioId, Guid currentUserId, CancellationToken ct = default);
    }
}

