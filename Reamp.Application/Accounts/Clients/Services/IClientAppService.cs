using Reamp.Application.Accounts.Clients.Dtos;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Accounts.Clients.Services
{
    public interface IClientAppService
    {
        // Create a new client
        Task<ClientDetailDto> CreateAsync(CreateClientDto dto, CancellationToken ct = default);

        // Update client agency/branch assignment
        Task<ClientDetailDto> UpdateAsync(Guid clientId, UpdateClientDto dto, CancellationToken ct = default);

        // Get client by ID
        Task<ClientDetailDto?> GetByIdAsync(Guid clientId, CancellationToken ct = default);

        // Get client by UserProfileId
        Task<ClientDetailDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default);

        // List clients by agency with pagination
        Task<IPagedList<ClientListDto>> ListByAgencyAsync(Guid agencyId, PageRequest pageRequest, CancellationToken ct = default);

        // Delete a client (soft delete)
        Task DeleteAsync(Guid clientId, CancellationToken ct = default);
    }
}

