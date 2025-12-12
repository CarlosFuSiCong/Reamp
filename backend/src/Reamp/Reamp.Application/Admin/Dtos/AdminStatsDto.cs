namespace Reamp.Application.Admin.Dtos
{
    public sealed class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveListings { get; set; }
        public int TotalOrders { get; set; }
        public int TotalStudios { get; set; }
        public List<ChartDataPoint> ChartData { get; set; } = new();
        public List<SystemAlert> Alerts { get; set; } = new();
    }

    public sealed class ChartDataPoint
    {
        public string Date { get; set; } = default!;
        public int Orders { get; set; }
        public int Listings { get; set; }
    }

    public sealed class SystemAlert
    {
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
