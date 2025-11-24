using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Accounts.Agencies.Services
{
    public interface IAgencyAppService
    {
        /// <summary>
        /// Create a new agency
        /// </summary>
        Task<AgencyDetailDto> CreateAsync(CreateAgencyDto dto, Guid currentUserId, CancellationToken ct = default);

        /// <summary>
        /// Update an existing agency
        /// </summary>
        Task<AgencyDetailDto> UpdateAsync(Guid agencyId, UpdateAgencyDto dto, CancellationToken ct = default);

        /// <summary>
        /// Get agency by ID
        /// </summary>
        Task<AgencyDetailDto?> GetByIdAsync(Guid agencyId, CancellationToken ct = default);

        /// <summary>
        /// Get agency by slug
        /// </summary>
        Task<AgencyDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default);

        /// <summary>
        /// List agencies with pagination
        /// </summary>
        Task<IPagedList<AgencyListDto>> ListAsync(PageRequest pageRequest, string? search = null, CancellationToken ct = default);

        /// <summary>
        /// Delete an agency (soft delete)
        /// </summary>
        Task DeleteAsync(Guid agencyId, CancellationToken ct = default);

        /// <summary>
        /// Check if agency exists by slug
        /// </summary>
        Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);
    }
}

