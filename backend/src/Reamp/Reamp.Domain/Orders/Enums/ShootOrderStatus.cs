using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Shoots.Enums
{
    public enum ShootOrderStatus
    {
        Placed = 1,
        Accepted = 2,
        Scheduled = 3,
        InProgress = 4,
        Completed = 5,
        Cancelled = 6
    }
}
