using Reamp.Domain.Accounts.Enums;
using System;

namespace Reamp.Application.Invitations.Dtos
{
    public sealed record InvitationDetailDto
    {
        public Guid Id { get; init; }
        public InvitationType Type { get; init; }
        public Guid TargetEntityId { get; init; }
        public string TargetEntityName { get; init; } = string.Empty;
        public Guid? TargetBranchId { get; init; }
        public string? TargetBranchName { get; init; }
        public string InviteeEmail { get; init; } = string.Empty;
        public int TargetRoleValue { get; init; }
        public string TargetRoleName { get; init; } = string.Empty;
        public InvitationStatus Status { get; init; }
        public Guid InvitedBy { get; init; }
        public string InvitedByName { get; init; } = string.Empty;
        public DateTime ExpiresAtUtc { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? RespondedAtUtc { get; init; }
        public bool IsExpired { get; init; }
    }
}
