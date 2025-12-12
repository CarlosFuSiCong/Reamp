using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IInvitationRepository
    {
        Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Invitation>> GetByInviteeEmailAsync(string email, CancellationToken ct = default);
        Task<List<Invitation>> GetPendingByInviteeEmailAsync(string email, CancellationToken ct = default);
        Task<List<Invitation>> GetByTargetEntityAsync(Guid entityId, InvitationType type, CancellationToken ct = default);
        Task<Invitation?> GetPendingInvitationAsync(Guid entityId, string email, InvitationType type, CancellationToken ct = default);
        Task AddAsync(Invitation invitation, CancellationToken ct = default);
        Task UpdateAsync(Invitation invitation, CancellationToken ct = default);
        Task DeleteAsync(Invitation invitation, CancellationToken ct = default);
    }
}
