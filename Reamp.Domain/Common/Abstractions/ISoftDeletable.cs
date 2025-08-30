using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.Abstractions
{
    public interface ISoftDeletable
    {
        DateTime? DeletedAtUtc { get; }
        bool IsDeleted { get; }

        void SoftDelete();
        void Restore();
    }
}