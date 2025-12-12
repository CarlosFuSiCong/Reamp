namespace Reamp.Application.Admin.Dtos
{
    public sealed class AdminStatsResponse
    {
        public AdminStatsDto Stats { get; set; } = default!;
        public List<ActivityDto> Activities { get; set; } = new();
    }
}
