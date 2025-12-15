using Reamp.Domain.Shoots.Enums;

namespace Reamp.Application.Orders.Dtos
{
    public class OrderListDto
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public Guid? StudioId { get; set; }
        public Guid ListingId { get; set; }
        public Guid? AssignedPhotographerId { get; set; }
        
        public string Title { get; set; } = string.Empty;
        // Display-friendly fields
        public string ListingTitle { get; set; } = string.Empty;
        public string? ListingAddress { get; set; }
        public string? StudioName { get; set; }
        public string? AgencyName { get; set; }
        
        public string Currency { get; set; } = "AUD";
        public decimal TotalAmount { get; set; }
        public ShootOrderStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ScheduledStartUtc { get; set; }
        public DateTime? ScheduledEndUtc { get; set; }
        public int TaskCount { get; set; }
    }
}





