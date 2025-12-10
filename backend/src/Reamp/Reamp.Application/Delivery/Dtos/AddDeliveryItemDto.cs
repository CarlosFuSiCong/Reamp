namespace Reamp.Application.Delivery.Dtos
{
    public sealed class AddDeliveryItemDto
    {
        public Guid MediaAssetId { get; set; }
        public string VariantName { get; set; } = default!;
        public int SortOrder { get; set; }
    }
}

