namespace Reamp.Application.Accounts.Agencies.Dtos
{
    public sealed class AgencyBranchDetailDto
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public AddressDto? Address { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}

