using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Common
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : AuditableEntity
    {
        protected readonly ApplicationDbContext _db;
        protected readonly DbSet<TEntity> _set;
        protected readonly ILogger _logger;

        protected BaseRepository(ApplicationDbContext db, ILogger logger)
        {
            _db = db;
            _set = db.Set<TEntity>();
            _logger = logger;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default)
        {
            IQueryable<TEntity> q = _set;
            if (asNoTracking) q = q.AsNoTracking();
            return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public virtual Task AddAsync(TEntity entity, CancellationToken ct = default)
            => _set.AddAsync(entity, ct).AsTask();

        public virtual void Remove(TEntity entity)
        {
            if (entity is ISoftDeletable soft && !soft.IsDeleted)
            {
                soft.SoftDelete();
                _logger.LogInformation("Soft-deleted entity {Entity} with Id {Id}", typeof(TEntity).Name, entity.Id);
            }
            else
            {
                _set.Remove(entity);
                _logger.LogWarning("Hard-deleted entity {Entity} with Id {Id}", typeof(TEntity).Name, entity.Id);
            }
        }
        protected async Task<IPagedList<T>> ToPagedListAsync<T>(
            IQueryable<T> query, PageRequest page, CancellationToken ct = default)
        {
            var total = await query.CountAsync(ct);
            var items = await query.Skip(page.Skip).Take(page.Take).ToListAsync(ct);
            return new PagedList<T>(items, total, page.Page, page.PageSize);
        }
    }
}