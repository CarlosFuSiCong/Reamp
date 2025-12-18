using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Common.Services
{
    /// <summary>
    /// Account query service for cross-aggregate queries.
    /// Note: This is a pragmatic architectural trade-off. Strict DDD would use Read Model (CQRS).
    /// </summary>
    public interface IAccountQueryService
    {
        Task<Agency?> GetAgencyAsync(Guid agencyId, CancellationToken ct = default);
        Task<AgencyBranch?> GetAgencyBranchAsync(Guid branchId, CancellationToken ct = default);
        Task<Studio?> GetStudioAsync(Guid studioId, CancellationToken ct = default);
        Task<UserProfile?> GetUserProfileAsync(Guid userProfileId, CancellationToken ct = default);
        
        Task<bool> AgencyExistsAsync(Guid agencyId, CancellationToken ct = default);
        Task<bool> StudioExistsAsync(Guid studioId, CancellationToken ct = default);
        
        Task<List<Client>> GetClientsByAgencyAsync(Guid agencyId, CancellationToken ct = default);
        Task<List<Staff>> GetStaffByStudioAsync(Guid studioId, StaffSkills? hasSkill = null, CancellationToken ct = default);
        Task<List<AgencyBranch>> GetBranchesByAgencyAsync(Guid agencyId, CancellationToken ct = default);
        
        Task<int> CountBranchesByAgencyAsync(Guid agencyId, CancellationToken ct = default);
    }
}

