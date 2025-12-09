using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Media.Enums
{
    public enum MediaProcessStatus
    {
        Unknown = 0,
        Uploaded = 1,
        Processing = 2,
        Ready = 3,
        Failed = 4
    }
}