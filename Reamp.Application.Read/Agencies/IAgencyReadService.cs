using Reamp.Application.Read.Shared;
using Reamp.Application.Read.Agencies.DTOs;

namespace Reamp.Application.Read.Agencies
{
    public interface IAgencyReadService
    {
        /// <summary>
        /// Get paginated list of agencies with optional search and filters
        /// </summary>
        Task<PageResult<AgencySummaryDto>> ListAsync(
            string? search = null,
            PageRequest? page = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get agency detail by ID
        /// </summary>
        Task<AgencyDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Get agency detail by slug
        /// </summary>
        Task<AgencyDetailDto?> GetDetailBySlugAsync(string slug, CancellationToken ct = default);

        /// <summary>
        /// Check if agency exists by slug
        /// </summary>
        Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);
    }
}

