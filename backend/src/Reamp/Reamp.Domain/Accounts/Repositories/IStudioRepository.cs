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
    public interface IStudioRepository : IRepository<Studio>
    {
        Task<Studio?> GetBySlugAsync(
            Slug slug,
            bool asNoTracking = true,
            CancellationToken ct = default);

        Task<bool> ExistsBySlugAsync(
            Slug slug,
            CancellationToken ct = default);

        Task<IPagedList<Studio>> ListAsync(
            PageRequest page,
            string? search = null,
            CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}