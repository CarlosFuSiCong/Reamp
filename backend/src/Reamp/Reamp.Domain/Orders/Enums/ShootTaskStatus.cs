using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Shoots.Enums
{
    public enum ShootTaskStatus
    {
        Pending = 1,
        Scheduled = 2,
        InProgress = 3,
        Done = 4,
        Cancelled = 5
    }
}
