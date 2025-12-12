using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Common.Services
{
    public interface IPermissionService
    {
        /// <summary>
        /// Check if user has specified agency role or higher
        /// </summary>
        Task<bool> HasAgencyRoleAsync(Guid userId, Guid agencyId, AgencyRole requiredRole, CancellationToken ct = default);

        /// <summary>
        /// Check if user has specified studio role or higher
        /// </summary>
        Task<bool> HasStudioRoleAsync(Guid userId, Guid studioId, StudioRole requiredRole, CancellationToken ct = default);

        /// <summary>
        /// Get user's agency role
        /// </summary>
        Task<AgencyRole?> GetAgencyRoleAsync(Guid userId, Guid agencyId, CancellationToken ct = default);

        /// <summary>
        /// Get user's studio role
        /// </summary>
        Task<StudioRole?> GetStudioRoleAsync(Guid userId, Guid studioId, CancellationToken ct = default);

        /// <summary>
        /// Check if user is owner of agency
        /// </summary>
        Task<bool> IsAgencyOwnerAsync(Guid userId, Guid agencyId, CancellationToken ct = default);

        /// <summary>
        /// Check if user is owner of studio
        /// </summary>
        Task<bool> IsStudioOwnerAsync(Guid userId, Guid studioId, CancellationToken ct = default);
    }
}
