using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Accounts.Studios.Dtos
{
    public sealed class AddressDto
    {
        public string Line1 { get; init; } = string.Empty;
        public string? Line2 { get; init; }
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Postcode { get; init; } = string.Empty;
        public string Country { get; init; } = "AU"; // ISO-2
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
    }
}