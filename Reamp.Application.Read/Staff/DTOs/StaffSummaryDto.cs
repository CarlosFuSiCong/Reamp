using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Read.Staff.DTOs
{
    public sealed class StaffSummaryDto
    {
        public Guid Id { get; set; }
        public Guid UserProfileId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public Guid StudioId { get; set; }
        public string? StudioName { get; set; }
        public StaffSkills Skills { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

