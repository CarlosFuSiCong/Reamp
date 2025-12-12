using Reamp.Domain.Accounts.Enums;
using System;

namespace Reamp.Application.Invitations.Dtos
{
    public sealed record InvitationListDto
    {
        public Guid Id { get; init; }
        public InvitationType Type { get; init; }
        public string TargetEntityName { get; init; } = string.Empty;
        public string TargetRoleName { get; init; } = string.Empty;
        public InvitationStatus Status { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public bool IsExpired { get; init; }
    }
}
