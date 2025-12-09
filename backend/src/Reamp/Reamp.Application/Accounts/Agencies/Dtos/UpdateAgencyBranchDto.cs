namespace Reamp.Application.Accounts.Agencies.Dtos
{
    public sealed class UpdateAgencyBranchDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public AddressDto? Address { get; set; }
    }
}



