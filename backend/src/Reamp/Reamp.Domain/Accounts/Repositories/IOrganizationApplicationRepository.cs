using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Repositories
{
    public interface IOrganizationApplicationRepository
    {
        Task<OrganizationApplication?> GetByIdAsync(Guid id, bool track = false, CancellationToken ct = default);
        
        Task<IPagedList<OrganizationApplication>> GetPagedAsync(
            PageRequest pageRequest,
            ApplicationStatus? status = null,
            ApplicationType? type = null,
            CancellationToken ct = default);
        
        Task<List<OrganizationApplication>> GetByApplicantAsync(
            Guid applicantUserId,
            CancellationToken ct = default);
        
        Task<bool> HasPendingApplicationAsync(
            Guid applicantUserId,
            ApplicationType? type = null,
            CancellationToken ct = default);
        
        Task AddAsync(OrganizationApplication application, CancellationToken ct = default);
        
        void Remove(OrganizationApplication application);
    }
}
