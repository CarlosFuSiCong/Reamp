using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Agent : AuditableEntity
    {
        public Guid UserProfileId { get; private set; }
        public Guid AgencyId { get; private set; }
        public Guid? AgencyBranchId { get; private set; }

        public AgencyRole Role { get; private set; }

        private Agent() { }

        public Agent(Guid userProfileId, Guid agencyId, AgencyRole role = AgencyRole.Agent, Guid? agencyBranchId = null)
        {
            if (userProfileId == Guid.Empty) throw new ArgumentException("UserProfileId required.");
            if (agencyId == Guid.Empty) throw new ArgumentException("AgencyId required.");

            UserProfileId = userProfileId;
            AgencyId = agencyId;
            AgencyBranchId = agencyBranchId;
            Role = role;
        }

        public void ChangeRole(AgencyRole role)
        {
            Role = role;
            Touch();
        }

        public void MoveToAgency(Guid agencyId, Guid? agencyBranchId = null)
        {
            if (agencyId == Guid.Empty) throw new ArgumentException("AgencyId required.");
            AgencyId = agencyId;
            AgencyBranchId = agencyBranchId;
            Touch();
        }

        public void ChangeBranch(Guid? agencyBranchId)
        {
            AgencyBranchId = agencyBranchId;
            Touch();
        }
    }
}

