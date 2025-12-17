using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IAgentRepository
    {
        Task<Entities.Agent?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Entities.Agent?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
        Task<List<Entities.Agent>> GetByAgencyIdAsync(Guid agencyId, CancellationToken ct = default);
        Task AddAsync(Entities.Agent agent, CancellationToken ct = default);
        Task UpdateAsync(Entities.Agent agent, CancellationToken ct = default);
        Task DeleteAsync(Entities.Agent agent, CancellationToken ct = default);
    }
}





