namespace Reamp.Application.Read.Agencies.DTOs
{
    public sealed class AgencySummaryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Slug { get; init; } = default!;
        public string? Description { get; init; }
        public Guid? LogoAssetId { get; init; }
        public string? LogoUrl { get; init; }
        public string ContactEmail { get; init; } = default!;
        public string ContactPhone { get; init; } = default!;
        public int BranchCount { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}



