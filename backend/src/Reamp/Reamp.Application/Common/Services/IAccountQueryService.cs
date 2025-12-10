using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Common.Services
{
    /// <summary>
    /// 账户关联查询服务 - 用于处理跨聚合的复杂查询
    /// 注意：这是为了实用性的架构权衡，在严格的 DDD 中应该使用 Read Model (CQRS)
    /// </summary>
    public interface IAccountQueryService
    {
        // Client 相关查询
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

