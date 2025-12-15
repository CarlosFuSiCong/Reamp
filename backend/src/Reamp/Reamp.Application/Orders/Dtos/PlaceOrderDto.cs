using System.ComponentModel.DataAnnotations;

namespace Reamp.Application.Orders.Dtos
{
    public class PlaceOrderDto
    {
        // AgencyId will be populated by the controller from the current user's agent record
        public Guid AgencyId { get; set; }

        // StudioId is optional - if not provided, order is published for studios to claim
        public Guid? StudioId { get; set; }

        [Required]
        public Guid ListingId { get; set; }

        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-character ISO code (e.g., AUD, USD)")]
        public string? Currency { get; set; }
    }
}



