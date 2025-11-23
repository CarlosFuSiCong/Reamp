using Reamp.Domain.Common.ValueObjects;
using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class CreateListingDto
    {
        public Guid OwnerAgencyId { get; set; }
        public Guid? AgentUserId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "AUD";
        public ListingType ListingType { get; set; }
        public PropertyType PropertyType { get; set; }
        public Address Address { get; set; } = default!;
    }
}