using Reamp.Application.Media.Dtos;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Media.Enums;

namespace Reamp.Application.Media.Services
{
    public interface IMediaAssetAppService
    {
        // Upload media (image or video)
        Task<MediaAssetDetailDto> UploadAsync(UploadMediaDto dto, Guid currentUserId, CancellationToken ct = default);

        // Get media asset by ID (with authorization)
        Task<MediaAssetDetailDto?> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default);

        // List media assets by studio (with authorization)
        Task<IPagedList<MediaAssetListDto>> ListByStudioAsync(
            Guid studioId,
            Guid currentUserId,
            MediaResourceType? resourceType = null,
            MediaProcessStatus? status = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);

        // Add variant to media asset (with authorization)
        Task<MediaAssetDetailDto> AddVariantAsync(Guid assetId, AddVariantDto dto, Guid currentUserId, CancellationToken ct = default);

        // Trigger processing for media asset (with authorization)
        Task TriggerProcessingAsync(Guid assetId, Guid currentUserId, CancellationToken ct = default);

        // Delete media asset (with ownership verification)
        Task DeleteAsync(Guid assetId, Guid studioId, Guid currentUserId, CancellationToken ct = default);
    }
}

