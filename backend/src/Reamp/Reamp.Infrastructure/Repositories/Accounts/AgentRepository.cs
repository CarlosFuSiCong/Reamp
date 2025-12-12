using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class AgentRepository : BaseRepository<Agent>, IAgentRepository
    {
        public AgentRepository(ApplicationDbContext db, ILogger<AgentRepository> logger)
            : base(db, logger) { }

        public async Task<Agent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await GetByIdAsync(id, true, ct);
        }

        public async Task<Agent?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            return await _set.FirstOrDefaultAsync(a => a.UserProfileId == userProfileId, ct);
        }

        public async Task<List<Agent>> GetByAgencyIdAsync(Guid agencyId, CancellationToken ct = default)
        {
            return await _set
                .Where(a => a.AgencyId == agencyId)
                .OrderBy(a => a.Role)
                .ThenBy(a => a.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public Task UpdateAsync(Agent agent, CancellationToken ct = default)
        {
            _set.Update(agent);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Agent agent, CancellationToken ct = default)
        {
            Remove(agent);
            return Task.CompletedTask;
        }
    }
}

