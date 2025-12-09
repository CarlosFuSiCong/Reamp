using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Listings.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Listings.Repositories
{
    public interface IListingRepository : IRepository<Listing>
    {
        new Task<Listing?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default);
        Task<Listing?> GetAggregateAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(Listing entity, CancellationToken ct = default);
    }
}