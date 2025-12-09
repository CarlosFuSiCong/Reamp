using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.Abstractions
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default);
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        void Remove(TEntity entity);
    }
}