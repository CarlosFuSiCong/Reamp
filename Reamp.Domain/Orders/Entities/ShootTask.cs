using Reamp.Domain.Shoots.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Shoots.Entities
{
    public sealed class ShootTask
    {
        public Guid Id { get; private set; }
        public Guid ShootOrderId { get; private set; }

        public ShootTaskType Type { get; private set; }     // Multiple flags allowed
        public ShootTaskStatus Status { get; private set; } = ShootTaskStatus.Pending;

        public Guid? AssigneeUserId { get; private set; }
        public DateTime? ScheduledStartUtc { get; private set; }
        public DateTime? ScheduledEndUtc { get; private set; }

        public decimal? Price { get; private set; }
        public string? Notes { get; private set; }

        private ShootTask() { }

        internal static ShootTask Create(Guid shootOrderId, ShootTaskType type, string? notes, decimal? price)
        {
            if (shootOrderId == Guid.Empty) throw new ArgumentException("ShootOrderId required");
            if (price is <= 0m) price = null;

            return new ShootTask
            {
                Id = Guid.NewGuid(),
                ShootOrderId = shootOrderId,
                Type = type,
                Status = ShootTaskStatus.Pending,
                Price = price,
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
            };
        }

        public void Schedule(DateTime startUtc, DateTime endUtc, Guid? assigneeUserId = null)
        {
            if (endUtc <= startUtc) throw new ArgumentException("End must be after start");
            ScheduledStartUtc = startUtc;
            ScheduledEndUtc = endUtc;
            AssigneeUserId = assigneeUserId;
            Status = ShootTaskStatus.Scheduled;
        }

        public void Start()
        {
            if (Status != ShootTaskStatus.Scheduled) throw new InvalidOperationException("Must be scheduled");
            Status = ShootTaskStatus.InProgress;
        }

        public void Done()
        {
            if (Status != ShootTaskStatus.InProgress && Status != ShootTaskStatus.Scheduled)
                throw new InvalidOperationException("Must be scheduled or in progress");
            Status = ShootTaskStatus.Done;
        }

        public void Cancel(string? reason = null)
        {
            Status = ShootTaskStatus.Cancelled;
            if (!string.IsNullOrWhiteSpace(reason))
                Notes = string.IsNullOrWhiteSpace(Notes) ? reason : $"{Notes}\n{reason}";
        }

        public void UpdatePrice(decimal? price)
        {
            if (price is <= 0m) price = null;
            Price = price;
        }

        public void UpdateNotes(string? notes) => Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}