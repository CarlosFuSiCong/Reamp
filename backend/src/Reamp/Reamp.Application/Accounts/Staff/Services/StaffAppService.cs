using Mapster;
using Reamp.Application.Accounts.Staff.Dtos;
using Reamp.Application.Common.Services;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Accounts.Staff.Services
{
    public sealed class StaffAppService : IStaffAppService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAccountQueryService _queryService;
        private readonly IUnitOfWork _unitOfWork;

        public StaffAppService(
            IStaffRepository staffRepository,
            IUserProfileRepository userProfileRepository,
            IAccountQueryService queryService,
            IUnitOfWork unitOfWork)
        {
            _staffRepository = staffRepository;
            _userProfileRepository = userProfileRepository;
            _queryService = queryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<StaffDetailDto> CreateAsync(CreateStaffDto dto, CancellationToken ct = default)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (userProfile == null || userProfile.DeletedAtUtc != null)
                throw new KeyNotFoundException($"UserProfile with ID {dto.UserProfileId} not found.");

            if (!await _queryService.StudioExistsAsync(dto.StudioId, ct))
                throw new KeyNotFoundException($"Studio with ID {dto.StudioId} not found.");

            var existingStaff = await _staffRepository.GetByUserProfileIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (existingStaff != null && existingStaff.DeletedAtUtc == null)
                throw new InvalidOperationException($"A staff member already exists for UserProfile {dto.UserProfileId}.");

            var staff = new Domain.Accounts.Entities.Staff(dto.UserProfileId, dto.StudioId, StudioRole.Member, dto.Skills);
            await _staffRepository.AddAsync(staff, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await BuildStaffDetailDtoAsync(staff, ct);
        }

        public async Task<StaffDetailDto> UpdateSkillsAsync(Guid staffId, UpdateStaffSkillsDto dto, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, asNoTracking: false, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Staff with ID {staffId} not found.");

            staff.SetSkills(dto.Skills);
            await _unitOfWork.SaveChangesAsync(ct);

            return await BuildStaffDetailDtoAsync(staff, ct);
        }

        public async Task<StaffDetailDto?> GetByIdAsync(Guid staffId, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, asNoTracking: true, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                return null;

            return await BuildStaffDetailDtoAsync(staff, ct);
        }

        public async Task<StaffDetailDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByUserProfileIdAsync(userProfileId, asNoTracking: true, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                return null;

            return await BuildStaffDetailDtoAsync(staff, ct);
        }

        public async Task<IPagedList<StaffListDto>> ListByStudioAsync(
            Guid studioId, 
            PageRequest pageRequest, 
            StaffSkills? hasSkill = null,
            CancellationToken ct = default)
        {
            if (!await _queryService.StudioExistsAsync(studioId, ct))
                throw new KeyNotFoundException($"Studio with ID {studioId} not found.");

            var pagedStaff = await _staffRepository.ListByStudioAsync(studioId, pageRequest, hasSkill, ct);

            var dtos = new List<StaffListDto>();
            foreach (var staff in pagedStaff.Items)
            {
                var userProfile = await _queryService.GetUserProfileAsync(staff.UserProfileId, ct);
                var studio = await _queryService.GetStudioAsync(staff.StudioId, ct);

                dtos.Add(new StaffListDto
                {
                    Id = staff.Id,
                    UserProfileId = staff.UserProfileId,
                    FirstName = userProfile?.FirstName ?? "",
                    LastName = userProfile?.LastName ?? "",
                    DisplayName = userProfile?.DisplayName ?? "",
                    StudioId = staff.StudioId,
                    StudioName = studio?.Name,
                    Skills = staff.Skills,
                    CreatedAtUtc = staff.CreatedAtUtc
                });
            }

            return new PagedList<StaffListDto>(
                dtos,
                pagedStaff.TotalCount,
                pagedStaff.Page,
                pagedStaff.PageSize
            );
        }

        public async Task DeleteAsync(Guid staffId, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, asNoTracking: false, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Staff with ID {staffId} not found.");

            _staffRepository.Remove(staff);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<StaffDetailDto> BuildStaffDetailDtoAsync(Domain.Accounts.Entities.Staff staff, CancellationToken ct)
        {
            var detailDto = staff.Adapt<StaffDetailDto>();
            var userProfile = await _queryService.GetUserProfileAsync(staff.UserProfileId, ct);
            var studio = await _queryService.GetStudioAsync(staff.StudioId, ct);
            
            detailDto.StudioName = studio?.Name;
            detailDto.FirstName = userProfile?.FirstName ?? "";
            detailDto.LastName = userProfile?.LastName ?? "";
            detailDto.DisplayName = userProfile?.DisplayName ?? "";
            
            return detailDto;
        }
    }
}
