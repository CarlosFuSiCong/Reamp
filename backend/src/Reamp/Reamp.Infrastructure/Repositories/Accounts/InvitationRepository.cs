using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Repositories.Accounts
{
    public sealed class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
    {
        public InvitationRepository(ApplicationDbContext db, ILogger<InvitationRepository> logger)
            : base(db, logger) { }

        public async Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await base.GetByIdAsync(id, true, ct);
        }

        public async Task<List<Invitation>> GetByInviteeEmailAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return await _set
                .Where(i => i.InviteeEmail == normalizedEmail)
                .OrderByDescending(i => i.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<List<Invitation>> GetPendingByInviteeEmailAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;
            
            return await _set
                .Where(i => i.InviteeEmail == normalizedEmail 
                    && i.Status == InvitationStatus.Pending
                    && i.ExpiresAtUtc > now)
                .OrderByDescending(i => i.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<List<Invitation>> GetByTargetEntityAsync(Guid entityId, InvitationType type, CancellationToken ct = default)
        {
            return await _set
                .Where(i => i.TargetEntityId == entityId && i.Type == type)
                .OrderByDescending(i => i.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<Invitation?> GetPendingInvitationAsync(Guid entityId, string email, InvitationType type, CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;

            return await _set
                .Where(i => i.TargetEntityId == entityId 
                    && i.InviteeEmail == normalizedEmail
                    && i.Type == type
                    && i.Status == InvitationStatus.Pending
                    && i.ExpiresAtUtc > now)
                .FirstOrDefaultAsync(ct);
        }

        public Task UpdateAsync(Invitation invitation, CancellationToken ct = default)
        {
            _set.Update(invitation);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Invitation invitation, CancellationToken ct = default)
        {
            Remove(invitation);
            return Task.CompletedTask;
        }
    }
}
