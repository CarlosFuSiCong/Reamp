using Reamp.Application.Read.Shared;
using Reamp.Application.Read.Staff.DTOs;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Read.Staff
{
    public interface IStaffReadService
    {
        // List staff by studio with pagination, search, and skill filter
        Task<PageResult<StaffSummaryDto>> ListByStudioAsync(
            Guid studioId,
            string? search,
            StaffSkills? hasSkill,
            PageRequest pageRequest,
            CancellationToken ct = default);

        // Get staff details by ID
        Task<StaffSummaryDto?> GetByIdAsync(Guid staffId, CancellationToken ct = default);

        // Get staff by UserProfileId
        Task<StaffSummaryDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    }
}



