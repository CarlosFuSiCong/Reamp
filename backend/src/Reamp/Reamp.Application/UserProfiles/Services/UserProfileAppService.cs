using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Application.UserProfiles.Dtos;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure;

namespace Reamp.Application.UserProfiles.Services
{
    public sealed class UserProfileAppService : IUserProfileAppService
    {
        private readonly IUserProfileRepository _profileRepo;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UserProfileAppService> _logger;

        public UserProfileAppService(
            IUserProfileRepository profileRepo,
            ApplicationDbContext dbContext,
            IUnitOfWork uow,
            ILogger<UserProfileAppService> logger)
        {
            _profileRepo = profileRepo;
            _dbContext = dbContext;
            _uow = uow;
            _logger = logger;
        }

        public async Task<UserProfileDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByIdAsync(id, false, true, ct);
            return profile?.Adapt<UserProfileDto>();
        }

        public async Task<UserProfileDto?> GetByApplicationUserIdAsync(Guid applicationUserId, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByApplicationUserIdAsync(applicationUserId, false, true, ct);
            return profile?.Adapt<UserProfileDto>();
        }

        public async Task<UserProfileDto> UpdateAsync(Guid id, UpdateUserProfileDto dto, CancellationToken ct = default)
        {
            var profile = await _profileRepo.GetByIdAsync(id, false, false, ct);
            if (profile == null)
                throw new KeyNotFoundException($"User profile with ID {id} not found");

            profile.UpdateBasicInfo(dto.FirstName, dto.LastName);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User profile {ProfileId} updated", id);
            return profile.Adapt<UserProfileDto>();
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
            return profile.Adapt<UserProfileDto>();
        }

        public async Task<List<UserProfileDto>> SearchAsync(string keyword, int limit = 20, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<UserProfileDto>();

            keyword = keyword.Trim().ToLower();
            limit = Math.Min(limit, 100); // Max 100 results

            var profiles = await _dbContext.Set<Domain.Accounts.Entities.UserProfile>()
                .AsNoTracking()
                .Where(p => p.DeletedAtUtc == null &&
                           (p.FirstName.ToLower().Contains(keyword) ||
                            p.LastName.ToLower().Contains(keyword)))
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .Take(limit)
                .ToListAsync(ct);

            return profiles.Adapt<List<UserProfileDto>>();
        }
    }
}

