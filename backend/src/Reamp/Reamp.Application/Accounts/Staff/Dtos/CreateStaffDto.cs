using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Accounts.Staff.Dtos
{
    public sealed class CreateStaffDto
    {
        public Guid UserProfileId { get; set; }
        public Guid StudioId { get; set; }
        public StudioRole Role { get; set; } = StudioRole.Member;
        public StaffSkills Skills { get; set; } = StaffSkills.None;
    }
}



