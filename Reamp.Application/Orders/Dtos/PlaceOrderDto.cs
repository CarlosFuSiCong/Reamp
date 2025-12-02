using System.ComponentModel.DataAnnotations;

namespace Reamp.Application.Orders.Dtos
{
    public class PlaceOrderDto
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        public Guid StudioId { get; set; }

        [Required]
        public Guid ListingId { get; set; }

        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-character ISO code (e.g., AUD, USD)")]
        public string? Currency { get; set; }
    }
}



