using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedAtUtc { get; }
        DateTime UpdatedAtUtc { get; }
        void MarkCreated();
        void MarkUpdated();
    }   
}

