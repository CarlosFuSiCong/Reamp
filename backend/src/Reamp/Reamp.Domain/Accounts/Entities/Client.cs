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
        public Guid UserProfileId { get; private set; }
        public Guid AgencyId { get; private set; }
        public Guid? AgencyBranchId { get; private set; }

        private Client() { }

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
