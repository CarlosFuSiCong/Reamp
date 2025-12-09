using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Domain.Accounts.Repositories
{
    // Repository for querying Staff entities
    public interface IStaffRepository : IRepository<Staff>
    {
        // Get staff by user profile ID
        Task<Staff?> GetByUserProfileIdAsync(Guid userProfileId, bool asNoTracking = true, CancellationToken ct = default);

        // List staff by studio with pagination
        Task<IPagedList<Staff>> ListByStudioAsync(
            Guid studioId,
            PageRequest pageRequest,
            StaffSkills? hasSkill = null,
            CancellationToken ct = default);

        // Check if an ApplicationUser (by ApplicationUserId) is a staff member of a studio
        Task<bool> IsApplicationUserStaffOfStudioAsync(Guid applicationUserId, Guid studioId, CancellationToken ct = default);
    }
}
