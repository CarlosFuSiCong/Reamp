using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Media.Entities;
using Reamp.Domain.Media.Enums;

namespace Reamp.Domain.Media.Repositories
{
    public interface IMediaAssetRepository : IRepository<MediaAsset>
    {
        // Find media asset by checksum within a specific studio (for deduplication)
        Task<MediaAsset?> FindByChecksumAsync(
            string checksumSha256,
            Guid studioId,
            CancellationToken ct = default);

        // List media assets by studio with optional filters
        Task<IPagedList<MediaAsset>> ListByStudioAsync(
            Guid studioId,
            MediaResourceType? resourceType = null,
            MediaProcessStatus? status = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);
    }
}

