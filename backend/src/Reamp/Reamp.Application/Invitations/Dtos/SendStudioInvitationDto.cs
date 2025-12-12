using Reamp.Domain.Accounts.Enums;
using System;

namespace Reamp.Application.Invitations.Dtos
{
    public sealed record SendStudioInvitationDto
    {
        public string InviteeEmail { get; init; } = string.Empty;
        public StudioRole TargetRole { get; init; }
    }
}
