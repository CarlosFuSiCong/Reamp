using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Delivery.Enums
{
    public enum AccessType
    {
        Public = 1,   // anyone with link
        Token = 2,   // link with token
        Private = 3    // per-recipient allowlist
    }
}
