using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.UserProfiles.Dtos
{
    public sealed class UserProfileDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public Guid? AvatarAssetId { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // Agency information (if user is an agent)
        public Guid? AgencyId { get; set; }
        public AgencyRole? AgencyRole { get; set; }

        // Studio information (if user is a studio staff)
        public Guid? StudioId { get; set; }
        public StudioRole? StudioRole { get; set; }
    }
}

