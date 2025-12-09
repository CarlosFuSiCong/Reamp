using Reamp.Domain.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Common.Entities
{
    public abstract class AuditableEntity : BaseEntity, IAuditableEntity, ISoftDeletable
    {
        public DateTime CreatedAtUtc { get; protected set; }
        public DateTime UpdatedAtUtc { get; protected set; }
        public DateTime? DeletedAtUtc { get; protected set; }

        public bool IsDeleted => DeletedAtUtc != null;

        public void MarkCreated()
        {
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
        }

        public void MarkUpdated()
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }

        protected void Touch() => MarkUpdated();

        public void SoftDelete()
        {
            if (DeletedAtUtc is null)
            {
                DeletedAtUtc = DateTime.UtcNow;
                Touch();
            }
        }

        public void Restore()
        {
            if (DeletedAtUtc is not null)
            {
                DeletedAtUtc = null;
                Touch();
            }
        }

        protected AuditableEntity()
        {
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
        }
    }
}