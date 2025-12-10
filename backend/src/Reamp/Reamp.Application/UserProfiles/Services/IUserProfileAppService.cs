using Reamp.Application.UserProfiles.Dtos;

namespace Reamp.Application.UserProfiles.Services
{
    public interface IUserProfileAppService
    {
        // Get user profile by ID
        Task<UserProfileDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

        // Get user profile by ApplicationUserId
        Task<UserProfileDto?> GetByApplicationUserIdAsync(Guid applicationUserId, CancellationToken ct = default);

        // Update user profile
        Task<UserProfileDto> UpdateAsync(Guid id, UpdateUserProfileDto dto, CancellationToken ct = default);

        // Update avatar
        Task<UserProfileDto> UpdateAvatarAsync(Guid id, Guid? avatarAssetId, CancellationToken ct = default);

        // Search users by name
        Task<List<UserProfileDto>> SearchAsync(string keyword, int limit = 20, CancellationToken ct = default);
    }
}

