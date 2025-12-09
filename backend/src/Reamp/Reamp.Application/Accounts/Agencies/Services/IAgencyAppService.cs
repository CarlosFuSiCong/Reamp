using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Accounts.Agencies.Services
{
    public interface IAgencyAppService
    {
        // Create a new agency
        Task<AgencyDetailDto> CreateAsync(CreateAgencyDto dto, Guid currentUserId, CancellationToken ct = default);

        // Update an existing agency
        Task<AgencyDetailDto> UpdateAsync(Guid agencyId, UpdateAgencyDto dto, CancellationToken ct = default);

        // Get agency by ID
        Task<AgencyDetailDto?> GetByIdAsync(Guid agencyId, CancellationToken ct = default);

        // Get agency by slug
        Task<AgencyDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default);

        // List agencies with pagination
        Task<IPagedList<AgencyListDto>> ListAsync(PageRequest pageRequest, string? search = null, CancellationToken ct = default);

        // Delete an agency (soft delete)
        Task DeleteAsync(Guid agencyId, CancellationToken ct = default);

        // Update agency logo
        Task<AgencyDetailDto> UpdateLogoAsync(Guid agencyId, Guid? logoAssetId, CancellationToken ct = default);

        // Check if agency exists by slug
        Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);

        // Branch Management

        // Create a new branch for an agency
        Task<AgencyBranchDetailDto> CreateBranchAsync(Guid agencyId, CreateAgencyBranchDto dto, Guid currentUserId, CancellationToken ct = default);

        // Update an existing branch
        Task<AgencyBranchDetailDto> UpdateBranchAsync(Guid agencyId, Guid branchId, UpdateAgencyBranchDto dto, CancellationToken ct = default);

        // Get branch by ID
        Task<AgencyBranchDetailDto?> GetBranchByIdAsync(Guid agencyId, Guid branchId, CancellationToken ct = default);

        // List branches for an agency
        Task<IReadOnlyList<AgencyBranchDetailDto>> ListBranchesAsync(Guid agencyId, CancellationToken ct = default);

        // Delete a branch (soft delete)
        Task DeleteBranchAsync(Guid agencyId, Guid branchId, CancellationToken ct = default);
    }
}

