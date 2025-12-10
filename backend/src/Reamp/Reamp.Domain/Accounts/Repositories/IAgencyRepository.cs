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
    public interface IAgencyRepository : IRepository<Agency>
    {
        Task<Agency?> GetBySlugAsync(
            Slug slug,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<bool> ExistsBySlugAsync(
            Slug slug,
            CancellationToken ct = default);

        Task<IPagedList<Agency>> ListAsync(
            PageRequest page,
            string? search = null,
            CancellationToken ct = default);

        Task<IReadOnlyList<AgencyBranch>> ListBranchesAsync(
            Guid agencyId,
            CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}