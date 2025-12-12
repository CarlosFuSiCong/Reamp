namespace Reamp.Application.Admin.Dtos
{
    public sealed class ActivityDto
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? User { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
