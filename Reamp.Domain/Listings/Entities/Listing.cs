using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reamp.Domain.Common.ValueObjects;
using Reamp.Domain.Listings.Enums;

namespace Reamp.Domain.Listings.Entities
{
    public sealed class Listing
    {
        public Guid Id { get; private set; }
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

        public Listing(string title, string description, decimal price,
                       ListingType type, PropertyType propertyType, Address address,
                       string currency = "AUD", Guid? agentUserId = null)
        {
            Title = title;
            Description = description;
            Price = price;
            ListingType = type;
            PropertyType = propertyType;
            Address = address;
            Currency = currency;
            Status = ListingStatus.Draft;

            if (agentUserId.HasValue)
            {
                AgentUserId = agentUserId.Value;
                var agentSnapshot = new ListingAgentSnapshot(
                    listingId: this.Id,
                    first: "Agent", last: "Default", email: "default@agent.com",
                    phone: "1234567890", team: "Agency", avatar: "default_avatar_url",
                    isPrimary: true, sortOrder: 0
                );
                AddAgent(agentSnapshot);
            }
        }

        public void AddMedia(ListingMediaRef media) => _mediaRefs.Add(media);

        public void AddAgent(ListingAgentSnapshot agent)
        {
            if (_agentSnapshots.Any(x => x.IsPrimary)) return;
            _agentSnapshots.Add(agent);
        }

        public void AssignAgent(Guid agentUserId, ListingAgentSnapshot snapshot)
        {
            if (snapshot.ListingId != this.Id)
                throw new InvalidOperationException("Agent snapshot must belong to this listing.");

            foreach (var a in _agentSnapshots) a.UnsetPrimary();
            snapshot.MakePrimary();
            _agentSnapshots.Add(snapshot);

            AgentUserId = agentUserId;
        }

        public void Publish()
        {
            if (!AgentUserId.HasValue)
                throw new InvalidOperationException("Cannot publish a listing without an assigned agent.");

            Status = ListingStatus.Active;
        }

        public void Archive() => Status = ListingStatus.Archived;
    }
}
