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
    }
}

