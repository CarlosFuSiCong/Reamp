using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Accounts.Staff.Dtos
{
    public sealed class CreateStaffDto
    {
        public Guid UserProfileId { get; set; }
        public Guid StudioId { get; set; }
        public StaffSkills Skills { get; set; } = StaffSkills.None;
    }
}



