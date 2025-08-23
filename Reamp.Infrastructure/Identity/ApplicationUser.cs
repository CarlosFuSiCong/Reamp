using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
        public DateTime? DeletedAtUtc { get; private set; }

        public bool IsDeleted => DeletedAtUtc.HasValue;

        public void SoftDelete()
        {
            if (!IsDeleted)
                DeletedAtUtc = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (IsDeleted)
                DeletedAtUtc = null;
        }
    }
}
