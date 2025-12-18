using Microsoft.Extensions.Logging;
using Reamp.Application.Listings.Dtos;
using Reamp.Domain.Accounts.Repositories;
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
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly IAgentRepository _agentRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ListingAppService> _logger;

        public ListingAppService(
            IListingRepository repo, 
            IUserProfileRepository userProfileRepo,
            IAgentRepository agentRepo,
            IUnitOfWork uow, 
            ILogger<ListingAppService> logger)
        {
            _repo = repo;
            _userProfileRepo = userProfileRepo;
            _agentRepo = agentRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Guid> CreateAsync(CreateListingDto dto, Guid applicationUserId, CancellationToken ct)
        {
            // Get UserProfile to find UserProfileId
            var userProfile = await _userProfileRepo.GetByApplicationUserIdAsync(applicationUserId, asNoTracking: true, ct: ct);
            if (userProfile == null)
            {
                _logger.LogWarning("User {ApplicationUserId} attempted to create listing but has no user profile", applicationUserId);
                throw new InvalidOperationException("User profile not found");
            }

            // Get Agent record using UserProfileId
            var agent = await _agentRepo.GetByUserProfileIdAsync(userProfile.Id, ct);
            if (agent == null)
            {
                _logger.LogWarning("UserProfile {UserProfileId} (ApplicationUser {ApplicationUserId}) attempted to create listing but has no agent record", 
                    userProfile.Id, applicationUserId);
                throw new InvalidOperationException("Agent record not found");
            }

            // Set ownerAgencyId and agentUserId
            dto.OwnerAgencyId = agent.AgencyId;
            dto.AgentUserId = userProfile.Id;  // Use UserProfileId, not ApplicationUserId

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
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.Publish();
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ArchiveAsync(Guid listingId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.Archive();
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task SetCoverAsync(Guid listingId, Guid mediaId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) ?? throw new InvalidOperationException("Listing not found.");
            listing.SetCover(mediaId);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task UpdateDetailsAsync(Guid listingId, UpdateListingDetailsDto dto, CancellationToken ct)
        {
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
        }

        public async Task<Guid> AddMediaAsync(Guid listingId, AddMediaDto dto, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            var mediaRef = listing.AddMedia(dto.MediaAssetId, dto.Role, dto.SortOrder);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);

            return mediaRef.Id;
        }

        public async Task RemoveMediaAsync(Guid listingId, Guid mediaRefId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            listing.RemoveMedia(mediaRefId);
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid listingId, CancellationToken ct)
        {
            var listing = await _repo.GetAggregateAsync(listingId, ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            _repo.Remove(listing);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task RestoreAsync(Guid listingId, CancellationToken ct)
        {
            var listing = await _repo.GetByIdAsync(listingId, asNoTracking: false, includeDeleted: true, ct: ct) 
                ?? throw new InvalidOperationException("Listing not found.");

            if (listing.DeletedAtUtc == null)
                throw new InvalidOperationException("Listing is not deleted.");

            listing.Restore();
            
            await _repo.UpdateAsync(listing, ct);
            await _uow.SaveChangesAsync(ct);
        }

    }
}