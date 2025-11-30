using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Persistence;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Staff> _set;

        public StaffRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = dbContext.Set<Staff>();
        }

        // IRepository<Staff> implementation
        public async Task<Staff?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var query = asNoTracking ? _set.AsNoTracking() : _set;
            // P1 Fix: Filter out soft-deleted staff to be consistent with other queries
            return await query.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAtUtc == null, ct);
        }

        public async Task AddAsync(Staff entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
        }

        public void Remove(Staff entity)
        {
            // P1 Fix: Use soft delete instead of hard delete to preserve audit trail
            // Staff inherits from AuditableEntity which provides soft delete functionality
            entity.SoftDelete();
        }

        // IStaffRepository specific methods
        public async Task<Staff?> GetByUserProfileIdAsync(Guid userProfileId, bool asNoTracking = true, CancellationToken ct = default)
        {
            var query = asNoTracking ? _set.AsNoTracking() : _set;
            return await query.FirstOrDefaultAsync(s => s.UserProfileId == userProfileId && s.DeletedAtUtc == null, ct);
        }

        public async Task<IPagedList<Staff>> ListByStudioAsync(
            Guid studioId,
            PageRequest pageRequest,
            StaffSkills? hasSkill = null,
            CancellationToken ct = default)
        {
            var query = _set.Where(s => s.StudioId == studioId && s.DeletedAtUtc == null);

            // P2 Fix: Filter by skill if specified (match ANY requested skill, not ALL)
            // Changed from == to != 0 to align with StaffAppService expectation
            // When multiple skills passed (e.g., Photographer | Videographer), should match staff with either skill
            if (hasSkill.HasValue && hasSkill.Value != StaffSkills.None)
            {
                query = query.Where(s => (s.Skills & hasSkill.Value) != 0);
            }

            var totalCount = await query.CountAsync(ct);

            // Normalize pagination
            var pageNumber = pageRequest.Page > 0 ? pageRequest.Page : 1;
            var pageSize = pageRequest.PageSize > 0 && pageRequest.PageSize <= 100 ? pageRequest.PageSize : 10;

            var items = await query
                .OrderBy(s => s.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedList<Staff>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> IsApplicationUserStaffOfStudioAsync(Guid applicationUserId, Guid studioId, CancellationToken ct = default)
        {
            // Join UserProfile to get UserProfileId from ApplicationUserId, then check Staff membership
            return await _set
                .Join(_dbContext.Set<UserProfile>(),
                      staff => staff.UserProfileId,
                      profile => profile.Id,
                      (staff, profile) => new { Staff = staff, Profile = profile })
                .AnyAsync(x => x.Profile.ApplicationUserId == applicationUserId 
                            && x.Staff.StudioId == studioId 
                            && x.Staff.DeletedAtUtc == null 
                            && x.Profile.DeletedAtUtc == null, ct);
        }
    }
}
