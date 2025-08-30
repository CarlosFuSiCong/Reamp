using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<Staff?> GetByUserProfileIdAsync(
            Guid profileId,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<IPagedList<Staff>> ListByStudioAsync(
            Guid studioId,
            PageRequest page,
            StaffSkills? hasSkill = null,
            CancellationToken ct = default);
    }
}