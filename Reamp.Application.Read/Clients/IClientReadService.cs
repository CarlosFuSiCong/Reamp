using Reamp.Application.Read.Clients.DTOs;
using Reamp.Application.Read.Shared;

namespace Reamp.Application.Read.Clients
{
    public interface IClientReadService
    {
        // List clients by agency with pagination and search
        Task<PageResult<ClientSummaryDto>> ListByAgencyAsync(
            Guid agencyId,
            string? search,
            PageRequest pageRequest,
            CancellationToken ct = default);

        // Get client details by ID
        Task<ClientSummaryDto?> GetByIdAsync(Guid clientId, CancellationToken ct = default);

        // Get client by UserProfileId
        Task<ClientSummaryDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);
    }
}



