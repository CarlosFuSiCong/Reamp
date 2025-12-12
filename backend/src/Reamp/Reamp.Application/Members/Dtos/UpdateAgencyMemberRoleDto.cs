using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Members.Dtos
{
    public sealed record UpdateAgencyMemberRoleDto
    {
        public AgencyRole NewRole { get; init; }
    }
}
