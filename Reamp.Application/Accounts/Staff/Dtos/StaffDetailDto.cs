using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Accounts.Staff.Dtos
{
    public sealed class StaffDetailDto
    {
        public Guid Id { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid StudioId { get; set; }
        public string? StudioName { get; set; }
        public StaffSkills Skills { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        // UserProfile information
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
    }
}



