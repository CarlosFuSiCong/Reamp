using Reamp.Domain.Accounts.Enums;
using System;

namespace Reamp.Application.Members.Dtos
{
    public sealed record AgencyMemberDto
    {
        public Guid Id { get; init; }
        public Guid UserProfileId { get; init; }
        public string DisplayName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public Guid? AvatarAssetId { get; init; }
        public AgencyRole Role { get; init; }
        public Guid? AgencyBranchId { get; init; }
        public string? AgencyBranchName { get; init; }
        public DateTime JoinedAtUtc { get; init; }
    }
}
