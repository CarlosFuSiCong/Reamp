using Microsoft.EntityFrameworkCore;
using Reamp.Application.Read.Shared;
using Reamp.Application.Read.Staff;
using Reamp.Application.Read.Staff.DTOs;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Infrastructure;

namespace Reamp.Infrastructure.Read.EF.Staff
{
    public sealed class EfStaffReadService : IStaffReadService
    {
        private readonly ApplicationDbContext _dbContext;

        public EfStaffReadService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResult<StaffSummaryDto>> ListByStudioAsync(
            Guid studioId,
            string? search,
            StaffSkills? hasSkill,
            PageRequest pageRequest,
            CancellationToken ct = default)
        {
            // Validate studio exists and is not deleted
            var studioExists = await _dbContext.Set<Studio>()
                .AnyAsync(s => s.Id == studioId && s.DeletedAtUtc == null, ct);

            if (!studioExists)
                throw new KeyNotFoundException($"Studio with ID {studioId} not found.");

            var query = _dbContext.Set<Domain.Accounts.Entities.Staff>()
                .AsNoTracking()
                .Where(s => s.StudioId == studioId && s.DeletedAtUtc == null);

            // Apply skill filter
            if (hasSkill.HasValue && hasSkill.Value != StaffSkills.None)
            {
                query = query.Where(s => (s.Skills & hasSkill.Value) != 0);
            }

            // Apply search filter (search by user profile name)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(s =>
                    _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Any(u => u.FirstName.ToLower().Contains(searchLower) ||
                                u.LastName.ToLower().Contains(searchLower))
                );
            }

            // Normalize page request
            var pageNumber = pageRequest.Page <= 0 ? 1 : pageRequest.Page;
            var pageSize = pageRequest.PageSize <= 0 ? 20 : (pageRequest.PageSize > PageRequest.MaxPageSize ? PageRequest.MaxPageSize : pageRequest.PageSize);

            var totalCount = await query.CountAsync(ct);

            // Apply sorting and pagination
            var sortBy = pageRequest.SortBy?.ToLower();
            query = sortBy switch
            {
                "createdat" => pageRequest.Desc 
                    ? query.OrderByDescending(s => s.CreatedAtUtc)
                    : query.OrderBy(s => s.CreatedAtUtc),
                _ => query.OrderByDescending(s => s.CreatedAtUtc) // default sort
            };

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StaffSummaryDto
                {
                    Id = s.Id,
                    UserProfileId = s.UserProfileId,
                    FirstName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "",
                    LastName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.LastName)
                        .FirstOrDefault() ?? "",
                    DisplayName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "",
                    StudioId = s.StudioId,
                    StudioName = _dbContext.Set<Studio>()
                        .Where(st => st.Id == s.StudioId)
                        .Select(st => st.Name)
                        .FirstOrDefault(),
                    Skills = s.Skills,
                    CreatedAtUtc = s.CreatedAtUtc
                })
                .ToListAsync(ct);

            return new PageResult<StaffSummaryDto>
            {
                Items = items,
                Total = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<StaffSummaryDto?> GetByIdAsync(Guid staffId, CancellationToken ct = default)
        {
            return await _dbContext.Set<Domain.Accounts.Entities.Staff>()
                .AsNoTracking()
                .Where(s => s.Id == staffId && s.DeletedAtUtc == null)
                .Select(s => new StaffSummaryDto
                {
                    Id = s.Id,
                    UserProfileId = s.UserProfileId,
                    FirstName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "",
                    LastName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.LastName)
                        .FirstOrDefault() ?? "",
                    DisplayName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "",
                    StudioId = s.StudioId,
                    StudioName = _dbContext.Set<Studio>()
                        .Where(st => st.Id == s.StudioId)
                        .Select(st => st.Name)
                        .FirstOrDefault(),
                    Skills = s.Skills,
                    CreatedAtUtc = s.CreatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<StaffSummaryDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            return await _dbContext.Set<Domain.Accounts.Entities.Staff>()
                .AsNoTracking()
                .Where(s => s.UserProfileId == userProfileId && s.DeletedAtUtc == null)
                .Select(s => new StaffSummaryDto
                {
                    Id = s.Id,
                    UserProfileId = s.UserProfileId,
                    FirstName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "",
                    LastName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.LastName)
                        .FirstOrDefault() ?? "",
                    DisplayName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == s.UserProfileId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "",
                    StudioId = s.StudioId,
                    StudioName = _dbContext.Set<Studio>()
                        .Where(st => st.Id == s.StudioId)
                        .Select(st => st.Name)
                        .FirstOrDefault(),
                    Skills = s.Skills,
                    CreatedAtUtc = s.CreatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}



