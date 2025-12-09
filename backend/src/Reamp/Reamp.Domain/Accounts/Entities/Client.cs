using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Client : AuditableEntity
    {
        public Guid UserProfileId { get; private set; }   // 1:1 到 UserProfile
        public Guid AgencyId { get; private set; }        // 必填
        public Guid? AgencyBranchId { get; private set; } // 可选

        private Client() { } // EF

        public Client(Guid userProfileId, Guid agencyId, Guid? agencyBranchId = null)
        {
            if (userProfileId == Guid.Empty) throw new ArgumentException("UserProfileId required.");
            if (agencyId == Guid.Empty) throw new ArgumentException("AgencyId required.");

            UserProfileId = userProfileId;
            AgencyId = agencyId;
            AgencyBranchId = agencyBranchId;
        }

        public void MoveToAgency(Guid agencyId, Guid? agencyBranchId = null)
        {
            if (agencyId == Guid.Empty) throw new ArgumentException("AgencyId required.");
            AgencyId = agencyId;
            AgencyBranchId = agencyBranchId;
            Touch();
        }
    }
}
