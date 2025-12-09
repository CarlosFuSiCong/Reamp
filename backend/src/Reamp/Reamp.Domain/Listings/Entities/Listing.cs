using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reamp.Domain.Common.Entities;
using Reamp.Domain.Common.ValueObjects;
using Reamp.Domain.Listings.Enums;

namespace Reamp.Domain.Listings.Entities
{
    public sealed class Listing : AuditableEntity
    {
        public Guid OwnerAgencyId { get; private set; }
        public Guid? AgentUserId { get; private set; }

        public string Title { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public decimal Price { get; private set; }
        public string Currency { get; private set; } = "AUD";

        public ListingType ListingType { get; private set; }
        public PropertyType PropertyType { get; private set; }
        public ListingStatus Status { get; private set; }

        public DateTime? AvailableFromUtc { get; private set; }

        public int Bedrooms { get; private set; }
        public int Bathrooms { get; private set; }
        public int ParkingSpaces { get; private set; }
        public double? FloorAreaSqm { get; private set; }
        public double? LandAreaSqm { get; private set; }
        public Address Address { get; private set; } = default!;

        private readonly List<ListingMediaRef> _mediaRefs = new();
        public IReadOnlyCollection<ListingMediaRef> MediaRefs => _mediaRefs.AsReadOnly();

        private readonly List<ListingAgentSnapshot> _agentSnapshots = new();
        public IReadOnlyCollection<ListingAgentSnapshot> AgentSnapshots => _agentSnapshots.AsReadOnly();

        private Listing() { }

        public Listing(
            Guid ownerAgencyId,
            string title,
            string description,
            decimal price,
            ListingType type,
            PropertyType propertyType,
            Address address,
            string currency = "AUD",
            Guid? agentUserId = null)
        {
            if (ownerAgencyId == Guid.Empty) throw new ArgumentException(nameof(ownerAgencyId));
            OwnerAgencyId = ownerAgencyId;

            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
            Title = title.Trim();
            Description = description ?? string.Empty;
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
            Price = price;

            ListingType = type;
            PropertyType = propertyType;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Currency = string.IsNullOrWhiteSpace(currency) ? "AUD" : currency.Trim();
            Status = ListingStatus.Draft;

            if (agentUserId.HasValue)
                AgentUserId = agentUserId.Value;
        }


        public void SetCover(Guid mediaId)
        {
            var target = _mediaRefs.FirstOrDefault(m => m.Id == mediaId) ?? throw new InvalidOperationException("Media not found.");
            foreach (var m in _mediaRefs) m.UnsetCover();
            target.SetCover();
        }

        public void SetMediaVisibility(Guid mediaId, bool isVisible)
        {
            var target = _mediaRefs.FirstOrDefault(m => m.Id == mediaId) ?? throw new InvalidOperationException("Media not found.");
            target.SetVisible(isVisible);
        }


        public void ReorderMedia(IReadOnlyDictionary<Guid, int> orders)
        {
            foreach (var m in _mediaRefs)
            {
                if (orders.TryGetValue(m.Id, out var sort)) m.UpdateSortOrder(sort);
            }
        }

        public void AddAgent(ListingAgentSnapshot agent)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (agent.ListingId != this.Id) throw new InvalidOperationException("Agent snapshot must belong to this listing.");
            agent.UnsetPrimary();
            _agentSnapshots.Add(agent);
        }

        public ListingAgentSnapshot CreateAgent(
            string first, string last, string email,
            string? phone = null, string? team = null, string? avatar = null,
            int sortOrder = 0)
        {
            var snap = new ListingAgentSnapshot(this.Id, first, last, email, phone, team, avatar, false, sortOrder);
            snap.UnsetPrimary();
            _agentSnapshots.Add(snap);
            return snap;
        }

        public void AssignAgent(Guid agentUserId, ListingAgentSnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.ListingId != this.Id) throw new InvalidOperationException("Agent snapshot must belong to this listing.");
            foreach (var a in _agentSnapshots) a.UnsetPrimary();
            snapshot.MakePrimary();
            if (!_agentSnapshots.Any(a => a.Id == snapshot.Id)) _agentSnapshots.Add(snapshot);
            AgentUserId = agentUserId;
        }

        public void UnassignPrimaryAgent()
        {
            foreach (var a in _agentSnapshots) a.UnsetPrimary();
            AgentUserId = null;
        }

        public void RemoveAgent(Guid agentSnapshotId)
        {
            var idx = _agentSnapshots.FindIndex(a => a.Id == agentSnapshotId);
            if (idx < 0) return;
            var wasPrimary = _agentSnapshots[idx].IsPrimary;
            _agentSnapshots.RemoveAt(idx);
            if (wasPrimary) AgentUserId = null;
        }

        public void ReorderAgents(IReadOnlyDictionary<Guid, int> orders)
        {
            foreach (var a in _agentSnapshots)
            {
                if (orders.TryGetValue(a.Id, out var sort)) a.UpdateSortOrder(sort);
            }
        }

        public void Publish()
        {
            if (!AgentUserId.HasValue) throw new InvalidOperationException("Cannot publish without a primary agent.");
            Status = ListingStatus.Active;
        }

        public void Archive() => Status = ListingStatus.Archived;

        public void UpdateDetails(
            string title,
            string description,
            decimal price,
            ListingType type,
            PropertyType propertyType,
            Address address,
            string? currency = null,
            int? bedrooms = null,
            int? bathrooms = null,
            int? parkingSpaces = null,
            double? floorAreaSqm = null,
            double? landAreaSqm = null,
            DateTime? availableFromUtc = null)
        {
            Title = string.IsNullOrWhiteSpace(title) ? Title : title.Trim();
            Description = description ?? string.Empty;
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
            Price = price;
            ListingType = type;
            PropertyType = propertyType;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            if (!string.IsNullOrWhiteSpace(currency)) Currency = currency!.Trim();
            if (bedrooms.HasValue) Bedrooms = bedrooms.Value;
            if (bathrooms.HasValue) Bathrooms = bathrooms.Value;
            if (parkingSpaces.HasValue) ParkingSpaces = parkingSpaces.Value;
            FloorAreaSqm = floorAreaSqm;
            LandAreaSqm = landAreaSqm;
            AvailableFromUtc = availableFromUtc;
        }
    }
}