using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Accounts.Agents.Dtos
{
    public sealed class AgentDetailDto
    {
        public Guid Id { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid AgencyId { get; set; }
        public string? AgencyName { get; set; }
        public Guid? AgencyBranchId { get; set; }
        public string? AgencyBranchName { get; set; }
        public AgencyRole Role { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        // UserProfile information
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}
