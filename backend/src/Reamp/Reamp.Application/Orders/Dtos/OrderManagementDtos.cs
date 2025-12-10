namespace Reamp.Application.Orders.Dtos
{
    public sealed class AssignPhotographerDto
    {
        public Guid PhotographerId { get; set; }
    }

    public sealed class SetScheduleDto
    {
        public DateTime ScheduledStartUtc { get; set; }
        public DateTime? ScheduledEndUtc { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class OrderFilterDto
    {
        public Guid? AgencyId { get; set; }
        public Guid? StudioId { get; set; }
        public Guid? ListingId { get; set; }
        public Guid? PhotographerId { get; set; }
        public Reamp.Domain.Shoots.Enums.ShootOrderStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}

