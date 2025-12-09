using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Media.Entities;
using Reamp.Domain.Media.Enums;
using Reamp.Domain.Media.Repositories;
using Reamp.Infrastructure.Persistence;

namespace Reamp.Infrastructure.Repositories.Media
{
    public sealed class MediaAssetRepository : IMediaAssetRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<MediaAsset> _set;

        public MediaAssetRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = dbContext.Set<MediaAsset>();
        }

        public async Task<MediaAsset?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var query = asNoTracking ? _set.AsNoTracking() : _set;
            return await query
                .Include(m => m.Variants)
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task AddAsync(MediaAsset entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
        }

        public void Remove(MediaAsset entity)
        {
            _set.Remove(entity);
        }

        public async Task<MediaAsset?> FindByChecksumAsync(
            string checksumSha256,
            Guid studioId,
            CancellationToken ct = default)
        {
            return await _set
                .Include(m => m.Variants)
                .FirstOrDefaultAsync(m => m.ChecksumSha256 == checksumSha256 && m.OwnerStudioId == studioId, ct);
        }

        public async Task<IPagedList<MediaAsset>> ListByStudioAsync(
            Guid studioId,
            MediaResourceType? resourceType = null,
            MediaProcessStatus? status = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            // Normalize pagination parameters to prevent ArgumentOutOfRangeException
            const int MaxPageSize = 100;
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var query = _set.Where(m => m.OwnerStudioId == studioId);

            if (resourceType.HasValue)
                query = query.Where(m => m.ResourceType == resourceType.Value);

            if (status.HasValue)
                query = query.Where(m => m.ProcessStatus == status.Value);

            query = query.OrderByDescending(m => m.CreatedAtUtc);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Include(m => m.Variants)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedList<MediaAsset>(items, totalCount, page, pageSize);
        }
    }
}

