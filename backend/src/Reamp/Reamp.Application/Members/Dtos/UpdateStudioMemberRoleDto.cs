using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Members.Dtos
{
    public sealed record UpdateStudioMemberRoleDto
    {
        public StudioRole NewRole { get; init; }
    }
}
