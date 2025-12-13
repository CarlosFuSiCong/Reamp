using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class OrganizationApplicationRepository : BaseRepository<OrganizationApplication>, IOrganizationApplicationRepository
    {
        public OrganizationApplicationRepository(ApplicationDbContext db, ILogger<OrganizationApplicationRepository> logger)
            : base(db, logger) { }

        public async Task<OrganizationApplication?> GetByIdAsync(Guid id, bool track = false, CancellationToken ct = default)
        {
            var query = _set.AsQueryable();
            if (!track) query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAtUtc == null, ct);
        }

        public async Task<PagedResult<OrganizationApplication>> GetPagedAsync(
            PageRequest pageRequest,
            ApplicationStatus? status = null,
            ApplicationType? type = null,
            CancellationToken ct = default)
        {
            var query = _set.AsNoTracking().Where(x => x.DeletedAtUtc == null);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (type.HasValue)
                query = query.Where(x => x.Type == type.Value);

            query = query.OrderByDescending(x => x.CreatedAtUtc);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .ToListAsync(ct);

            return new PagedResult<OrganizationApplication>(items, total, pageRequest.Page, pageRequest.PageSize);
        }

        public async Task<List<OrganizationApplication>> GetByApplicantAsync(
            Guid applicantUserId,
            CancellationToken ct = default)
        {
            return await _set
                .AsNoTracking()
                .Where(x => x.ApplicantUserId == applicantUserId && x.DeletedAtUtc == null)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<bool> HasPendingApplicationAsync(
            Guid applicantUserId,
            ApplicationType? type = null,
            CancellationToken ct = default)
        {
            var query = _set.Where(x =>
                x.ApplicantUserId == applicantUserId &&
                (x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.UnderReview) &&
                x.DeletedAtUtc == null);

            if (type.HasValue)
                query = query.Where(x => x.Type == type.Value);

            return await query.AnyAsync(ct);
        }

        public async Task AddAsync(OrganizationApplication application, CancellationToken ct = default)
        {
            await _set.AddAsync(application, ct);
        }

        public void Remove(OrganizationApplication application)
        {
            _set.Remove(application);
        }
    }
}
