using Reamp.Domain.Common.ValueObjects;

namespace Reamp.Application.Accounts.Agencies.Dtos
{
    public sealed class CreateAgencyBranchDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public AddressDto? Address { get; set; }
    }

    public sealed class AddressDto
    {
        public string Line1 { get; set; } = default!;
        public string? Line2 { get; set; }
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Postcode { get; set; } = default!;
        public string Country { get; set; } = default!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public Address ToValueObject()
        {
            return new Address(
                Line1,
                City,
                State,
                Postcode,
                Country,
                Line2,
                Latitude,
                Longitude
            );
        }
    }
}

