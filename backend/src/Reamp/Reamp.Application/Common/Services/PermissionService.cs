using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Infrastructure;

namespace Reamp.Application.Common.Services
{
    public sealed class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAgentRepository _agentRepo;
        private readonly IStaffRepository _staffRepo;

        public PermissionService(
            ApplicationDbContext db,
            IAgentRepository agentRepo,
            IStaffRepository staffRepo)
        {
            _db = db;
            _agentRepo = agentRepo;
            _staffRepo = staffRepo;
        }

        public async Task<bool> HasAgencyRoleAsync(Guid userId, Guid agencyId, AgencyRole requiredRole, CancellationToken ct = default)
        {
            var userRole = await GetAgencyRoleAsync(userId, agencyId, ct);
            if (userRole == null) return false;
            
            // Higher role number = higher permission
            return userRole.Value >= requiredRole;
        }

        public async Task<bool> HasStudioRoleAsync(Guid userId, Guid studioId, StudioRole requiredRole, CancellationToken ct = default)
        {
            var userRole = await GetStudioRoleAsync(userId, studioId, ct);
            if (userRole == null) return false;
            
            // Higher role number = higher permission
            return userRole.Value >= requiredRole;
        }

        public async Task<AgencyRole?> GetAgencyRoleAsync(Guid userId, Guid agencyId, CancellationToken ct = default)
        {
            // Get UserProfile by ApplicationUserId
            var userProfile = await _db.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(up => up.ApplicationUserId == userId && up.DeletedAtUtc == null, ct);

            if (userProfile == null) return null;

            // Get Agent record directly by UserProfileId (one-to-one relationship)
            var agent = await _agentRepo.GetByUserProfileIdAsync(userProfile.Id, ct);
            if (agent == null || agent.DeletedAtUtc != null || agent.AgencyId != agencyId)
                return null;

            return agent.Role;
        }

        public async Task<StudioRole?> GetStudioRoleAsync(Guid userId, Guid studioId, CancellationToken ct = default)
        {
            // Get UserProfile by ApplicationUserId
            var userProfile = await _db.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(up => up.ApplicationUserId == userId && up.DeletedAtUtc == null, ct);

            if (userProfile == null) return null;

            // Get Staff record
            var staff = await _staffRepo.GetByUserProfileIdAsync(userProfile.Id, asNoTracking: true, ct);
            if (staff == null || staff.DeletedAtUtc != null || staff.StudioId != studioId)
                return null;

            return staff.Role;
        }

        public async Task<bool> IsAgencyOwnerAsync(Guid userId, Guid agencyId, CancellationToken ct = default)
        {
            var role = await GetAgencyRoleAsync(userId, agencyId, ct);
            return role == AgencyRole.Owner;
        }

        public async Task<bool> IsStudioOwnerAsync(Guid userId, Guid studioId, CancellationToken ct = default)
        {
            var role = await GetStudioRoleAsync(userId, studioId, ct);
            return role == StudioRole.Owner;
        }
    }
}
