namespace Reamp.Application.Delivery.Dtos
{
    public sealed class UpdateDeliveryPackageDto
    {
        public string? Title { get; set; }
        public bool? WatermarkEnabled { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
    }
}

