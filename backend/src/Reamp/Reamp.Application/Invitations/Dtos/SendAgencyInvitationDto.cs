using Reamp.Domain.Accounts.Enums;
using System;

namespace Reamp.Application.Invitations.Dtos
{
    public sealed record SendAgencyInvitationDto
    {
        public string InviteeEmail { get; init; } = string.Empty;
        public AgencyRole TargetRole { get; init; }
        public Guid? AgencyBranchId { get; init; }
    }
}
