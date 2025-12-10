using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IAgencyBranchRepository : IRepository<AgencyBranch>
    {
        Task<AgencyBranch?> GetBySlugAsync(
            Guid agencyId,
            Slug slug,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<IPagedList<AgencyBranch>> ListAsync(
            Guid agencyId,
            PageRequest page,
            string? search = null,
            CancellationToken ct = default);

        Task<AgencyBranch?> GetByIdAndAgencyAsync(
            Guid id,
            Guid agencyId,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<int> CountByAgencyAsync(Guid agencyId, CancellationToken ct = default);

        Task<List<AgencyBranch>> GetByAgencyAsync(
            Guid agencyId,
            bool asNoTracking = true,
            CancellationToken ct = default);
    }
}