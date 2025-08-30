using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
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
    public sealed class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        public StaffRepository(ApplicationDbContext db, ILogger<StaffRepository> logger)
            : base(db, logger) { }

        public async Task<Staff?> GetByUserProfileIdAsync(Guid profileId, bool asNoTracking = true, CancellationToken ct = default)
        {
            var q = _set.AsQueryable();
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(s => s.UserProfileId == profileId, ct);
        }

        public async Task<IPagedList<Staff>> ListByStudioAsync(Guid studioId, PageRequest page, StaffSkills? hasSkill = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(s => s.StudioId == studioId);

            if (hasSkill is StaffSkills skill && skill != StaffSkills.None)
            {
                q = q.Where(s => (s.Skills & skill) != 0);
            }

            q = q.OrderByDescending(s => s.CreatedAtUtc);
            return await ToPagedListAsync(q, page, ct);
        }
    }
}