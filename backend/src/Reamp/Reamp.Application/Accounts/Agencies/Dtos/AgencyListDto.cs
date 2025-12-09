namespace Reamp.Application.Accounts.Agencies.Dtos
{
    public sealed class AgencyListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public Guid? LogoAssetId { get; set; }
        public string? LogoUrl { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public int BranchCount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}



