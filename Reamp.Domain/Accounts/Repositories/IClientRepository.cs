using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> GetByUserProfileIdAsync(
            Guid userProfileId,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<IPagedList<Client>> ListByAgencyAsync(
            Guid agencyId,
            PageRequest page,
            CancellationToken ct = default);
    }
}