using Reamp.Application.Applications.Dtos;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Application.Applications.Services
{
    public interface IApplicationService
    {
        Task<Guid> SubmitAgencyApplicationAsync(Guid applicantUserId, SubmitAgencyApplicationDto dto, CancellationToken ct = default);
        
        Task<Guid> SubmitStudioApplicationAsync(Guid applicantUserId, SubmitStudioApplicationDto dto, CancellationToken ct = default);
        
        Task<IPagedList<ApplicationListDto>> GetApplicationsAsync(
            PageRequest pageRequest,
            ApplicationStatus? status = null,
            ApplicationType? type = null,
            CancellationToken ct = default);
        
        Task<List<ApplicationListDto>> GetMyApplicationsAsync(Guid userId, CancellationToken ct = default);
        
        Task<ApplicationDetailDto> GetApplicationDetailAsync(Guid applicationId, CancellationToken ct = default);
        
        Task ReviewApplicationAsync(Guid applicationId, Guid reviewedBy, ReviewApplicationDto dto, CancellationToken ct = default);
        
        Task CancelApplicationAsync(Guid applicationId, Guid userId, CancellationToken ct = default);
    }
}
