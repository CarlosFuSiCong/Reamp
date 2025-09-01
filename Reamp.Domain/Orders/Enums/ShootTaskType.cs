using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Shoots.Enums
{
    [Flags]
    public enum ShootTaskType
    {
        None = 0,
        Photography = 1 << 0,
        Video = 1 << 1,
        Floorplan = 1 << 2,
        VR360 = 1 << 3,
        Drone = 1 << 4,
        Other = 1 << 5
    }
}