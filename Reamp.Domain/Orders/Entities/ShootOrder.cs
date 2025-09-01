using Reamp.Domain.Common.Entities;
using Reamp.Domain.Shoots.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Shoots.Entities
{
    public sealed class ShootOrder : AuditableEntity
    {
        public Guid AgencyId { get; private set; }   // Ordering agency
        public Guid StudioId { get; private set; }   // Executing studio
        public Guid ListingId { get; private set; }  // Linked listing

        public string Currency { get; private set; } = "AUD";
        public decimal TotalAmount { get; private set; }

        public ShootOrderStatus Status { get; private set; } = ShootOrderStatus.Placed;
        public Guid CreatedBy { get; private set; }

        private readonly List<ShootTask> _tasks = new();
        public IReadOnlyCollection<ShootTask> Tasks => _tasks.AsReadOnly();

        private ShootOrder() { }

        public static ShootOrder Place(Guid agencyId, Guid studioId, Guid listingId, Guid createdBy, string currency = "AUD")
        {
            if (agencyId == Guid.Empty) throw new ArgumentException("AgencyId required");
            if (studioId == Guid.Empty) throw new ArgumentException("StudioId required");
            if (listingId == Guid.Empty) throw new ArgumentException("ListingId required");
            if (createdBy == Guid.Empty) throw new ArgumentException("CreatedBy required");

            return new ShootOrder
            {
                AgencyId = agencyId,
                StudioId = studioId,
                ListingId = listingId,
                CreatedBy = createdBy,
                Currency = string.IsNullOrWhiteSpace(currency) ? "AUD" : currency.Trim().ToUpperInvariant()
            };
        }

        public ShootTask AddTask(ShootTaskType type, string? notes = null, decimal? price = null)
        {
            EnsureNotFinal();
            var task = ShootTask.Create(Id == Guid.Empty ? Guid.NewGuid() : Id, type, notes, price);
            _tasks.Add(task);
            RecalculateTotal();
            Touch();
            return task;
        }

        public void RemoveTask(Guid taskId)
        {
            EnsureNotFinal();
            var removed = _tasks.RemoveAll(t => t.Id == taskId);
            if (removed == 0) throw new InvalidOperationException("Task not found");
            RecalculateTotal();
            Touch();
        }

        public void ClearTasks()
        {
            EnsureNotFinal();
            _tasks.Clear();
            TotalAmount = 0m;
            Touch();
        }

        // Status transitions
        public void Accept()
        {
            if (Status != ShootOrderStatus.Placed) throw new InvalidOperationException("Must be placed");
            if (_tasks.Count == 0) throw new InvalidOperationException("No tasks");
            Status = ShootOrderStatus.Accepted; Touch();
        }

        public void MarkScheduled()
        {
            if (Status != ShootOrderStatus.Accepted && Status != ShootOrderStatus.Scheduled)
                throw new InvalidOperationException("Must be accepted first");
            Status = ShootOrderStatus.Scheduled; Touch();
        }

        public void Start()
        {
            if (Status != ShootOrderStatus.Scheduled) throw new InvalidOperationException("Must be scheduled");
            Status = ShootOrderStatus.InProgress; Touch();
        }

        public void Complete()
        {
            if (Status != ShootOrderStatus.InProgress && Status != ShootOrderStatus.Scheduled)
                throw new InvalidOperationException("Must be in progress/scheduled");
            Status = ShootOrderStatus.Completed; Touch();
        }

        public void Cancel(string reason)
        {
            if (Status == ShootOrderStatus.Completed) throw new InvalidOperationException("Cannot cancel completed");
            if (Status == ShootOrderStatus.Cancelled) return;
            Status = ShootOrderStatus.Cancelled; Touch();
        }

        private void RecalculateTotal() => TotalAmount = _tasks.Sum(t => t.Price ?? 0m);

        private void EnsureNotFinal()
        {
            if (Status is ShootOrderStatus.Completed or ShootOrderStatus.Cancelled)
                throw new InvalidOperationException("Order is final");
        }
    }
}