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

        public string? Currency { get; set; }
    }
}



