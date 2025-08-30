using Microsoft.Extensions.Logging;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Common
{
    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EfUnitOfWork> _logger;

        public EfUnitOfWork(ApplicationDbContext db, ILogger<EfUnitOfWork> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                var affected = await _db.SaveChangesAsync(ct);
                _logger.LogInformation("SaveChangesAsync committed {AffectedRows} row(s).", affected);
                return affected;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveChangesAsync failed.");
                throw;
            }
        }
    }
}