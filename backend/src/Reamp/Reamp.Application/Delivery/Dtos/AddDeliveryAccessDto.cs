using Reamp.Domain.Delivery.Enums;

namespace Reamp.Application.Delivery.Dtos
{
    public sealed class AddDeliveryAccessDto
    {
        public AccessType Type { get; set; } = AccessType.Public;
        public string? RecipientEmail { get; set; }
        public string? RecipientName { get; set; }
        public int? MaxDownloads { get; set; }
        public string? Password { get; set; } // Plain text password (will be hashed)
    }
}

