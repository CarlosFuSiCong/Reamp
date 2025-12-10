using Reamp.Domain.Delivery.Enums;

namespace Reamp.Application.Delivery.Dtos
{
    public sealed class DeliveryPackageListDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ListingId { get; set; }
        public string Title { get; set; } = default!;
        public DeliveryStatus Status { get; set; }
        public int ItemCount { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

