using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Staff : AuditableEntity
    {
        public Guid UserProfileId { get; private set; }
        public Guid StudioId { get; private set; }

        public StudioRole Role { get; private set; }
        public StaffSkills Skills { get; private set; }

        private Staff() { } // EF

        public Staff(Guid userProfileId, Guid studioId, StudioRole role = StudioRole.Member, StaffSkills skills = StaffSkills.None)
        {
            if (userProfileId == Guid.Empty) throw new ArgumentException("UserProfileId required.");
            if (studioId == Guid.Empty) throw new ArgumentException("StudioId required.");

            UserProfileId = userProfileId;
            StudioId = studioId;
            Role = role;
            Skills = skills;
        }

        public void ChangeRole(StudioRole role)
        {
            Role = role;
            Touch();
        }

        public void ChangeStudio(Guid studioId)
        {
            if (studioId == Guid.Empty) throw new ArgumentException("StudioId required.");
            StudioId = studioId;
            Touch();
        }

        public void SetSkills(StaffSkills skills) { Skills = skills; Touch(); }
        public void AddSkills(StaffSkills skills) { Skills |= skills; Touch(); }
        public void RemoveSkills(StaffSkills skills) { Skills &= ~skills; Touch(); }
        public void ClearSkills() { Skills = StaffSkills.None; Touch(); }
        public bool HasSkill(StaffSkills skill) => (Skills & skill) != 0;
    }
}