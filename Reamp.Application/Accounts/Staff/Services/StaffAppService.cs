using Microsoft.EntityFrameworkCore;
using Reamp.Application.Accounts.Staff.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure;

namespace Reamp.Application.Accounts.Staff.Services
{
    public sealed class StaffAppService : IStaffAppService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public StaffAppService(
            IStaffRepository staffRepository,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork unitOfWork,
            ApplicationDbContext dbContext)
        {
            _staffRepository = staffRepository;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<StaffDetailDto> CreateAsync(CreateStaffDto dto, CancellationToken ct = default)
        {
            // Validate UserProfile exists
            var userProfile = await _userProfileRepository.GetByIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (userProfile == null || userProfile.DeletedAtUtc != null)
                throw new KeyNotFoundException($"UserProfile with ID {dto.UserProfileId} not found.");

            // Validate Studio exists and is not deleted
            var studio = await _dbContext.Set<Studio>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == dto.StudioId && s.DeletedAtUtc == null, ct);
            
            if (studio == null)
                throw new KeyNotFoundException($"Studio with ID {dto.StudioId} not found.");

            // Check if staff already exists for this UserProfile
            var existingStaff = await _staffRepository.GetByUserProfileIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (existingStaff != null && existingStaff.DeletedAtUtc == null)
                throw new InvalidOperationException($"A staff member already exists for UserProfile {dto.UserProfileId}.");

            var staff = new Domain.Accounts.Entities.Staff(dto.UserProfileId, dto.StudioId, dto.Skills);
            await _staffRepository.AddAsync(staff, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToDetailDtoAsync(staff, ct);
        }

        public async Task<StaffDetailDto> UpdateSkillsAsync(Guid staffId, UpdateStaffSkillsDto dto, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, asNoTracking: false, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Staff with ID {staffId} not found.");

            staff.SetSkills(dto.Skills);
            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToDetailDtoAsync(staff, ct);
        }

        public async Task<StaffDetailDto?> GetByIdAsync(Guid staffId, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, asNoTracking: true, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                return null;

            return await MapToDetailDtoAsync(staff, ct);
        }

        public async Task<StaffDetailDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByUserProfileIdAsync(userProfileId, asNoTracking: true, ct);
            if (staff == null || staff.DeletedAtUtc != null)
                return null;

            return await MapToDetailDtoAsync(staff, ct);
        }

        public async Task<IPagedList<StaffListDto>> ListByStudioAsync(
            Guid studioId, 
            PageRequest pageRequest, 
            StaffSkills? hasSkill = null,
            CancellationToken ct = default)
        {
            // Validate Studio exists
            var studioExists = await _dbContext.Set<Studio>()
                .AnyAsync(s => s.Id == studioId && s.DeletedAtUtc == null, ct);

            if (!studioExists)
                throw new KeyNotFoundException($"Studio with ID {studioId} not found.");

            var pagedStaff = await _staffRepository.ListByStudioAsync(studioId, pageRequest, hasSkill, ct);

            var staffList = await _dbContext.Set<Domain.Accounts.Entities.Staff>()
                .AsNoTracking()
                .Where(s => s.StudioId == studioId && s.DeletedAtUtc == null)
                .Where(s => !hasSkill.HasValue || (s.Skills & hasSkill.Value) != 0)
                .OrderByDescending(s => s.CreatedAtUtc)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(s => new
                {
                    Staff = s,
                    UserProfile = _dbContext.Set<UserProfile>().FirstOrDefault(u => u.Id == s.UserProfileId),
                    Studio = _dbContext.Set<Studio>().FirstOrDefault(st => st.Id == s.StudioId)
                })
                .ToListAsync(ct);

            var dtos = staffList.Select(item => new StaffListDto
            {
                Id = item.Staff.Id,
                UserProfileId = item.Staff.UserProfileId,
                FirstName = item.UserProfile?.FirstName ?? "",
                LastName = item.UserProfile?.LastName ?? "",
                DisplayName = item.UserProfile?.DisplayName ?? "",
                StudioId = item.Staff.StudioId,
                StudioName = item.Studio?.Name,
                Skills = item.Staff.Skills,
                CreatedAtUtc = item.Staff.CreatedAtUtc
            }).ToList();

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

        private async Task<StaffDetailDto> MapToDetailDtoAsync(Domain.Accounts.Entities.Staff staff, CancellationToken ct)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(staff.UserProfileId, asNoTracking: true, ct);
            var studio = await _dbContext.Set<Studio>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == staff.StudioId, ct);

            return new StaffDetailDto
            {
                Id = staff.Id,
                UserProfileId = staff.UserProfileId,
                StudioId = staff.StudioId,
                StudioName = studio?.Name,
                Skills = staff.Skills,
                CreatedAtUtc = staff.CreatedAtUtc,
                UpdatedAtUtc = staff.UpdatedAtUtc,
                FirstName = userProfile?.FirstName ?? "",
                LastName = userProfile?.LastName ?? "",
                DisplayName = userProfile?.DisplayName ?? ""
            };
        }
    }
}



