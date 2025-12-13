using Mapster;
using Microsoft.Extensions.Logging;
using Reamp.Application.UserProfiles.Dtos;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.UserProfiles.Services
{
    public sealed class UserProfileAppService : IUserProfileAppService
    {
        private readonly IUserProfileRepository _profileRepo;
        private readonly IAgentRepository _agentRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UserProfileAppService> _logger;

        public UserProfileAppService(
            IUserProfileRepository profileRepo,
            IAgentRepository agentRepo,
            IStaffRepository staffRepo,
            IUnitOfWork uow,
            ILogger<UserProfileAppService> logger)
        {
            _profileRepo = profileRepo;
            _agentRepo = agentRepo;
            _staffRepo = staffRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<UserProfileDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByIdAsync(id, false, true, ct);
            if (profile == null) return null;

            var dto = profile.Adapt<UserProfileDto>();
            await EnrichWithRoleInfoAsync(dto, id, ct);
            return dto;
        }

        public async Task<UserProfileDto?> GetByApplicationUserIdAsync(Guid applicationUserId, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByApplicationUserIdAsync(applicationUserId, false, true, ct);
            if (profile == null) return null;

            var dto = profile.Adapt<UserProfileDto>();
            await EnrichWithRoleInfoAsync(dto, profile.Id, ct);
            return dto;
        }

        public async Task<UserProfileDto> UpdateAsync(Guid id, UpdateUserProfileDto dto, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByIdAsync(id, false, false, ct);
            if (profile == null)
                throw new KeyNotFoundException($"User profile with ID {id} not found");

            profile.UpdateBasicInfo(dto.FirstName, dto.LastName);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User profile {ProfileId} updated", id);
            var result = profile.Adapt<UserProfileDto>();
            await EnrichWithRoleInfoAsync(result, id, ct);
            return result;
        }

        public async Task<UserProfileDto> UpdateAvatarAsync(Guid id, Guid? avatarAssetId, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByIdAsync(id, false, false, ct);
            if (profile == null)
                throw new KeyNotFoundException($"User profile with ID {id} not found");

            if (avatarAssetId.HasValue && avatarAssetId.Value != Guid.Empty)
            {
                profile.SetAvatarAsset(avatarAssetId.Value);
            }
            else
            {
                profile.ClearAvatar();
            }

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User profile {ProfileId} avatar updated", id);
            var result = profile.Adapt<UserProfileDto>();
            await EnrichWithRoleInfoAsync(result, id, ct);
            return result;
        }

        public async Task<List<UserProfileDto>> SearchAsync(string keyword, int limit = 20, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<UserProfileDto>();

            keyword = keyword.Trim();
            limit = Math.Min(limit, 100);

            var profiles = await _profileRepo.SearchAsync(keyword, limit, ct);
            var dtos = profiles.Adapt<List<UserProfileDto>>();

            // Enrich all profiles with role info
            foreach (var dto in dtos)
            {
                var profile = profiles.First(p => p.Id == dto.Id);
                await EnrichWithRoleInfoAsync(dto, profile.Id, ct);
            }

            return dtos;
        }

        private async Task EnrichWithRoleInfoAsync(UserProfileDto dto, Guid userProfileId, CancellationToken ct)
        {
            // Check if user is an agent
            var agent = await _agentRepo.GetByUserProfileIdAsync(userProfileId, ct);
            if (agent != null)
            {
                dto.AgencyId = agent.AgencyId;
                dto.AgencyRole = agent.Role;
            }

            // Check if user is a studio staff
            var staff = await _staffRepo.GetByUserProfileIdAsync(userProfileId, true, ct);
            if (staff != null)
            {
                dto.StudioId = staff.StudioId;
                dto.StudioRole = staff.Role;
            }
        }
    }
}

