using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Orders.Enums
{
    public enum ShootOrderStatus
    {
        Placed = 1,              // Order placed, waiting for Studio to accept
        Accepted = 2,            // Accepted by Studio
        Scheduled = 3,           // Shoot scheduled (staff assigned)
        InProgress = 4,          // Shoot in progress
        AwaitingDelivery = 5,    // Shoot completed, waiting for delivery upload
        AwaitingConfirmation = 6, // Delivery uploaded, waiting for Agent confirmation
        Completed = 7,           // Completed
        Cancelled = 8            // Cancelled
    }
}
