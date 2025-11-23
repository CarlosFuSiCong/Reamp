using Reamp.Domain.Common.ValueObjects;
using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Listings.Dtos
{
    public sealed class UpdateListingDetailsDto
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public ListingType ListingType { get; set; }
        public PropertyType PropertyType { get; set; }
        public Address Address { get; set; } = default!;
        public string? Currency { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? ParkingSpaces { get; set; }
        public double? FloorAreaSqm { get; set; }
        public double? LandAreaSqm { get; set; }
        public DateTime? AvailableFromUtc { get; set; }
    }
}