using Reamp.Application.Listings.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Services
{
    public interface IListingAppService
    {
        Task<Guid> CreateAsync(CreateListingDto dto, CancellationToken ct);
        Task<Guid> CreateAgentAsync(Guid listingId, CreateListingAgentDto dto, CancellationToken ct);
        Task AssignAgentAsync(Guid listingId, Guid agentUserId, Guid agentSnapshotId, CancellationToken ct);
        Task UnassignPrimaryAgentAsync(Guid listingId, CancellationToken ct);
        Task RemoveAgentAsync(Guid listingId, Guid agentSnapshotId, CancellationToken ct);
        Task ReorderAgentsAsync(Guid listingId, ReorderAgentsDto dto, CancellationToken ct);
        Task ReorderMediaAsync(Guid listingId, ReorderMediaDto dto, CancellationToken ct);
        Task SetMediaVisibilityAsync(Guid listingId, SetMediaVisibilityDto dto, CancellationToken ct);
        Task PublishAsync(Guid listingId, CancellationToken ct);
        Task ArchiveAsync(Guid listingId, CancellationToken ct);
        Task UpdateDetailsAsync(Guid listingId, UpdateListingDetailsDto dto, CancellationToken ct);
        Task SetCoverAsync(Guid listingId, Guid mediaId, CancellationToken ct);
        
        // Media Management
        Task<Guid> AddMediaAsync(Guid listingId, AddMediaDto dto, CancellationToken ct);
        Task RemoveMediaAsync(Guid listingId, Guid mediaRefId, CancellationToken ct);
        
        // Soft Delete Management
        Task DeleteAsync(Guid listingId, CancellationToken ct);
        Task RestoreAsync(Guid listingId, CancellationToken ct);
    }
}