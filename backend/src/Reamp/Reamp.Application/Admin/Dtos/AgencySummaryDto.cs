namespace Reamp.Application.Admin.Dtos
{
    public sealed class AgencySummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
