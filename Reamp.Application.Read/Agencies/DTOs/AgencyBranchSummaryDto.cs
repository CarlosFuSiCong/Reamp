namespace Reamp.Application.Read.Agencies.DTOs
{
    public sealed class AgencyBranchSummaryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Slug { get; init; } = default!;
        public string? Description { get; init; }
        public string ContactEmail { get; init; } = default!;
        public string ContactPhone { get; init; } = default!;
        public AddressDto? Address { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }

    public sealed class AddressDto
    {
        public string Line1 { get; init; } = default!;
        public string? Line2 { get; init; }
        public string City { get; init; } = default!;
        public string State { get; init; } = default!;
        public string Postcode { get; init; } = default!;
        public string Country { get; init; } = default!;
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
    }
}

