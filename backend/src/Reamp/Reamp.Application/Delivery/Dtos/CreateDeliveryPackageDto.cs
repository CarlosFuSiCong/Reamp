using Reamp.Domain.Delivery.Enums;

namespace Reamp.Application.Delivery.Dtos
{
    public sealed class CreateDeliveryPackageDto
    {
        public Guid OrderId { get; set; }
        public Guid ListingId { get; set; }
        public string Title { get; set; } = default!;
        public bool WatermarkEnabled { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
    }
}

