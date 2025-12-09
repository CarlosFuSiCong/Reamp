using System.ComponentModel.DataAnnotations;

namespace Reamp.Application.Orders.Dtos
{
    public class CancelOrderDto
    {
        [StringLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}



