using Reamp.Application.Accounts.Staff.Dtos;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Accounts.Staff.Services
{
    public interface IStaffAppService
    {
        // Create a new staff member
        Task<StaffDetailDto> CreateAsync(CreateStaffDto dto, CancellationToken ct = default);

        // Update staff skills
        Task<StaffDetailDto> UpdateSkillsAsync(Guid staffId, UpdateStaffSkillsDto dto, CancellationToken ct = default);

        // Get staff by ID
        Task<StaffDetailDto?> GetByIdAsync(Guid staffId, CancellationToken ct = default);

        // Get staff by UserProfileId
        Task<StaffDetailDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);

        // List staff by studio with pagination and optional skill filter
        Task<IPagedList<StaffListDto>> ListByStudioAsync(Guid studioId, PageRequest pageRequest, StaffSkills? hasSkill = null, CancellationToken ct = default);

        // Delete a staff member (soft delete)
        Task DeleteAsync(Guid staffId, CancellationToken ct = default);
    }
}

