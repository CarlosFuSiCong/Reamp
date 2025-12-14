using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Enums
{
    public enum UserRole : int
    {
        None = 0,
        Client = 1,
        Agent = 2,
        Staff = 3,
        Admin = 4
    }
}

