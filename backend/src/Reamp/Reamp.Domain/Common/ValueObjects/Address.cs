using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.ValueObjects
{
    public sealed class Address
    {
        private static readonly Regex CountryRegex = new("^[A-Z]{2}$", RegexOptions.Compiled);

        public string Line1 { get; private set; } = string.Empty;
        public string? Line2 { get; private set; }
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string Postcode { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;    // ISO 3166-1 alpha-2, e.g. "AU"
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        private Address() { } // EF

        public Address(string line1, string city, string state, string postcode, string country,
                       string? line2 = null, double? latitude = null, double? longitude = null)
        {
            if (string.IsNullOrWhiteSpace(line1)) throw new ArgumentException("Address.Line1 is required.");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("Address.City is required.");
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("Address.State is required.");
            if (string.IsNullOrWhiteSpace(postcode)) throw new ArgumentException("Address.Postcode is required.");
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Address.Country is required.");

            Line1 = line1.Trim();
            Line2 = string.IsNullOrWhiteSpace(line2) ? null : line2.Trim();
            City = city.Trim();
            State = state.Trim();
            Postcode = postcode.Trim();

            Country = country.Trim().ToUpperInvariant();
            if (!CountryRegex.IsMatch(Country))
                throw new ArgumentException("Address.Country must be ISO 3166-1 alpha-2, e.g. 'AU'.");

            if (latitude is < -90 or > 90) throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
            if (longitude is < -180 or > 180) throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

            Latitude = latitude;
            Longitude = longitude;
        }

        public Address With(string? line1 = null, string? city = null, string? state = null,
                            string? postcode = null, string? country = null, string? line2 = null,
                            double? latitude = null, double? longitude = null)
            => new Address(line1 ?? Line1, city ?? City, state ?? State, postcode ?? Postcode,
                           (country ?? Country), line2 ?? Line2, latitude ?? Latitude, longitude ?? Longitude);
    }
}