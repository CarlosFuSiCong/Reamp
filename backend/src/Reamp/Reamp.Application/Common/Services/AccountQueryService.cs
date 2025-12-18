using Microsoft.EntityFrameworkCore;
using Reamp.Application.Common.Services;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Infrastructure;

namespace Reamp.Application.Common.Services
{
    /// <summary>
    /// Account query service implementation.
    /// Encapsulates cross-aggregate query logic to avoid Application Services directly depending on DbContext.
    /// </summary>
    public sealed class AccountQueryService : IAccountQueryService
    {
        private readonly ApplicationDbContext _db;

        public AccountQueryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Agency?> GetAgencyAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _db.Set<Agency>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);
        }

        public async Task<AgencyBranch?> GetAgencyBranchAsync(Guid branchId, CancellationToken ct = default)
        {
            return await _db.Set<AgencyBranch>()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.DeletedAtUtc == null, ct);
        }

        public async Task<Studio?> GetStudioAsync(Guid studioId, CancellationToken ct = default)
        {
            return await _db.Set<Studio>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studioId && s.DeletedAtUtc == null, ct);
        }

        public async Task<UserProfile?> GetUserProfileAsync(Guid userProfileId, CancellationToken ct = default)
        {
            return await _db.Set<UserProfile>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userProfileId && u.DeletedAtUtc == null, ct);
        }

        public async Task<bool> AgencyExistsAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _db.Set<Agency>()
                .AnyAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);
        }

        public async Task<bool> StudioExistsAsync(Guid studioId, CancellationToken ct = default)
        {
            return await _db.Set<Studio>()
                .AnyAsync(s => s.Id == studioId && s.DeletedAtUtc == null, ct);
        }

        public async Task<List<Client>> GetClientsByAgencyAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _db.Set<Client>()
                .AsNoTracking()
                .Where(c => c.AgencyId == agencyId && c.DeletedAtUtc == null)
                .ToListAsync(ct);
        }

        public async Task<List<Staff>> GetStaffByStudioAsync(Guid studioId, StaffSkills? hasSkill = null, CancellationToken ct = default)
        {
            var query = _db.Set<Staff>()
                .AsNoTracking()
                .Where(s => s.StudioId == studioId && s.DeletedAtUtc == null);

            if (hasSkill.HasValue)
            {
                query = query.Where(s => (s.Skills & hasSkill.Value) != 0);
            }

            return await query.ToListAsync(ct);
        }

        public async Task<List<AgencyBranch>> GetBranchesByAgencyAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _db.Set<AgencyBranch>()
                .AsNoTracking()
                .Where(b => b.AgencyId == agencyId && b.DeletedAtUtc == null)
                .OrderBy(b => b.Name)
                .ToListAsync(ct);
        }

        public async Task<int> CountBranchesByAgencyAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _db.Set<AgencyBranch>()
                .CountAsync(b => b.AgencyId == agencyId && b.DeletedAtUtc == null, ct);
        }
    }
}

