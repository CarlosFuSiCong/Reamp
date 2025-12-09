using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Listings.Entities
{
    public sealed class ListingAgentSnapshot
    {
        public Guid Id { get; private set; }
        public Guid ListingId { get; private set; }
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string? PhoneNumber { get; private set; }
        public string? TeamOrOfficeName { get; private set; }
        public string? AvatarUrl { get; private set; }
        public bool IsPrimary { get; private set; }
        public int SortOrder { get; private set; }

        private ListingAgentSnapshot() { }

        public ListingAgentSnapshot(
            Guid listingId,
            string first,
            string last,
            string email,
            string? phone = null,
            string? team = null,
            string? avatar = null,
            bool isPrimary = false,
            int sortOrder = 0)
        {
            Id = Guid.NewGuid();
            ListingId = listingId;
            FirstName = first;
            LastName = last;
            Email = email;
            PhoneNumber = phone;
            TeamOrOfficeName = team;
            AvatarUrl = avatar;
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
        }

        public void MakePrimary() => IsPrimary = true;
        public void UnsetPrimary() => IsPrimary = false;
        public void UpdateSortOrder(int sortOrder)
        {
            if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder));
            SortOrder = sortOrder;
        }
    }
}