using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext db, ILogger<ClientRepository> logger)
            : base(db, logger) { }

        public async Task<Client?> GetByUserProfileIdAsync(Guid userProfileId, bool asNoTracking = true, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(c => c.UserProfileId == userProfileId, ct);
        }

        public async Task<IPagedList<Client>> ListByAgencyAsync(Guid agencyId, PageRequest page, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking()
                        .Where(c => c.AgencyId == agencyId)
                        .OrderByDescending(c => c.CreatedAtUtc);

            return await ToPagedListAsync(q, page, ct);
        }
    }
}