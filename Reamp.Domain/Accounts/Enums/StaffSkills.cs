using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Enums
{
    [Flags]
    public enum StaffSkills
    {
        None = 0,
        Photographer = 1 << 0, // 1
        Videographer = 1 << 1, // 2
        VRMaker = 1 << 2, // 4
    }
}