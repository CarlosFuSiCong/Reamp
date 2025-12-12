using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Accounts.Agents.Dtos
{
    public sealed class CreateAgentDto
    {
        public Guid UserProfileId { get; set; }
        public Guid AgencyId { get; set; }
        public Guid? AgencyBranchId { get; set; }
        public AgencyRole Role { get; set; } = AgencyRole.Agent;
    }
}
