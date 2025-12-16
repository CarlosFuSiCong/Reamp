using Reamp.Domain.Orders.Enums;

namespace Reamp.Application.Orders.Dtos
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public Guid? StudioId { get; set; }
        public Guid ListingId { get; set; }
        public Guid? AssignedPhotographerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Currency { get; set; } = "AUD";
        public decimal TotalAmount { get; set; }
        public ShootOrderStatus Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ScheduledStartUtc { get; set; }
        public DateTime? ScheduledEndUtc { get; set; }
        public string? SchedulingNotes { get; set; }
        public string? CancellationReason { get; set; }
        public List<TaskDetailDto> Tasks { get; set; } = new();
    }

    public class TaskDetailDto
    {
        public Guid Id { get; set; }
        public ShootTaskType Type { get; set; }
        public ShootTaskStatus Status { get; set; }
        public decimal? Price { get; set; }
        public string? Notes { get; set; }
        public DateTime? ScheduledStartUtc { get; set; }
        public DateTime? ScheduledEndUtc { get; set; }
        public Guid? AssigneeUserId { get; set; }
    }
}



