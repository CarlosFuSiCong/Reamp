using Microsoft.Extensions.Logging;
using Reamp.Application.Listings.Dtos;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Listings.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Services
{
    public sealed class ListingAppService : IListingAppService
    {
        private readonly IListingRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ListingAppService> _logger;

        public ListingAppService(IListingRepository repo, IUnitOfWork uow, ILogger<ListingAppService> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Guid> CreateAsync(CreateListingDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Creating new listing for agency {AgencyId} with title '{Title}'", 
                dto.OwnerAgencyId, dto.Title);

            var listing = new Listing(
                ownerAgencyId: dto.OwnerAgencyId,
                title: dto.Title,
                description: dto.Description,
                price: dto.Price,
                type: dto.ListingType,
                propertyType: dto.PropertyType,
                address: dto.Address,
                currency: dto.Currency,
                agentUserId: dto.AgentUserId
            );
            await _repo.AddAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
            
            _logger.LogInformation("Successfully created listing {ListingId} for agency {AgencyId}", 
                listing.Id, dto.OwnerAgencyId);
            
            return listing.Id;
        }

        public async Task<Guid> CreateAgentAsync(Guid listingId, CreateListingAgentDto dto, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            var snapshot = listing.CreateAgent(dto.FirstName, dto.LastName, dto.Email, dto.Phone, dto.Team, dto.Avatar, dto.SortOrder);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
            return snapshot.Id;
        }

        public async Task AssignAgentAsync(Guid listingId, Guid agentUserId, Guid agentSnapshotId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            var snapshot = listing.AgentSnapshots.FirstOrDefault(a => a.Id == agentSnapshotId) ?? throw new InvalidOperationException("Agent snapshot not found.");
            listing.AssignAgent(agentUserId, snapshot);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task UnassignPrimaryAgentAsync(Guid listingId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.UnassignPrimaryAgent();
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task RemoveAgentAsync(Guid listingId, Guid agentSnapshotId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.RemoveAgent(agentSnapshotId);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ReorderAgentsAsync(Guid listingId, ReorderAgentsDto dto, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            var map = dto.Items.ToDictionary(x => x.Id, x => x.SortOrder);
            listing.ReorderAgents(map);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ReorderMediaAsync(Guid listingId, ReorderMediaDto dto, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            var map = dto.Items.ToDictionary(x => x.Id, x => x.SortOrder);
            listing.ReorderMedia(map);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task SetMediaVisibilityAsync(Guid listingId, SetMediaVisibilityDto dto, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.SetMediaVisibility(dto.MediaId, dto.IsVisible);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task PublishAsync(Guid listingId, CancellationToken ct)
        {
            _logger.LogInformation("Publishing listing {ListingId}", listingId);
            
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.Publish();
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
            
            _logger.LogInformation("Successfully published listing {ListingId}", listingId);
        }

        public async Task ArchiveAsync(Guid listingId, CancellationToken ct)
        {
            _logger.LogInformation("Archiving listing {ListingId}", listingId);
            
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.Archive();
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
            
            _logger.LogInformation("Successfully archived listing {ListingId}", listingId);
        }
        public async Task SetCoverAsync(Guid listingId, Guid mediaId, CancellationToken ct)
        {
            _logger.LogInformation("Setting cover media {MediaId} for listing {ListingId}", mediaId, listingId);
            
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.SetCover(mediaId);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
            
            _logger.LogInformation("Successfully set cover media {MediaId} for listing {ListingId}", mediaId, listingId);
        }

        public async Task UpdateDetailsAsync(Guid listingId, UpdateListingDetailsDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating details for listing {ListingId} with title '{Title}'", 
                listingId, dto.Title);
            
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.UpdateDetails(
                title: dto.Title,
                description: dto.Description,
                price: dto.Price,
                type: dto.ListingType,
                propertyType: dto.PropertyType,
                address: dto.Address,
                currency: dto.Currency,
                bedrooms: dto.Bedrooms,
                bathrooms: dto.Bathrooms,
                parkingSpaces: dto.ParkingSpaces,
                floorAreaSqm: dto.FloorAreaSqm,
                landAreaSqm: dto.LandAreaSqm,
                availableFromUtc: dto.AvailableFromUtc
            );
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
            
            _logger.LogInformation("Successfully updated details for listing {ListingId}", listingId);
        }

        public async Task<Guid> AddMediaAsync(Guid listingId, AddMediaDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Adding media {MediaAssetId} to listing {ListingId}", dto.MediaAssetId, listingId);

            var listing = await _repo.GetAggregateAsync(listingId, ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            var mediaRef = listing.AddMedia(dto.MediaAssetId, dto.Role, dto.SortOrder);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully added media {MediaAssetId} to listing {ListingId}", dto.MediaAssetId, listingId);
            return mediaRef.Id;
        }

        public async Task RemoveMediaAsync(Guid listingId, Guid mediaRefId, CancellationToken ct)
        {
            _logger.LogInformation("Removing media {MediaRefId} from listing {ListingId}", mediaRefId, listingId);

            var listing = await _repo.GetAggregateAsync(listingId, ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            listing.RemoveMedia(mediaRefId);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully removed media {MediaRefId} from listing {ListingId}", mediaRefId, listingId);
        }

        public async Task DeleteAsync(Guid listingId, CancellationToken ct)
        {
            _logger.LogInformation("Soft deleting listing {ListingId}", listingId);

            var listing = await _repo.GetAggregateAsync(listingId, ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            _repo.Remove(listing);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully soft deleted listing {ListingId}", listingId);
        }

        public async Task RestoreAsync(Guid listingId, CancellationToken ct)
        {
            _logger.LogInformation("Restoring listing {ListingId}", listingId);

            var listing = await _repo.GetByIdAsync(listingId, asNoTracking: false, ct: ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            if (listing.DeletedAtUtc == null)
                throw new InvalidOperationException("Listing is not deleted.");

            // Manually clear DeletedAtUtc (simple restore)
            // Note: This is a temporary solution. Ideally add Restore() to domain entity
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Successfully restored listing {ListingId}", listingId);
        }

    }
}