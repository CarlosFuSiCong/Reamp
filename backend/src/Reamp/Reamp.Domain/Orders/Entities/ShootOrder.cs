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
        public Guid? StudioId { get; private set; }  // Executing studio (nullable for marketplace orders)
        public Guid ListingId { get; private set; }  // Linked listing
        public Guid? AssignedPhotographerId { get; private set; }  // Assigned photographer (Staff)

        public string Title { get; private set; } = string.Empty;
        public string Currency { get; private set; } = "AUD";
        public decimal TotalAmount { get; private set; }

        public ShootOrderStatus Status { get; private set; } = ShootOrderStatus.Placed;
        public Guid CreatedBy { get; private set; }
        public string? CancellationReason { get; private set; }

        // Scheduling
        public DateTime? ScheduledStartUtc { get; private set; }
        public DateTime? ScheduledEndUtc { get; private set; }
        public string? SchedulingNotes { get; private set; }

        private readonly List<ShootTask> _tasks = new();
        public IReadOnlyCollection<ShootTask> Tasks => _tasks.AsReadOnly();

        private ShootOrder() { }

        public static ShootOrder Place(Guid agencyId, Guid? studioId, Guid listingId, Guid createdBy, string title, string currency = "AUD")
        {
            if (agencyId == Guid.Empty) throw new ArgumentException("AgencyId required");
            // StudioId is optional - can be null for marketplace orders
            if (studioId.HasValue && studioId.Value == Guid.Empty) throw new ArgumentException("StudioId cannot be empty Guid");
            if (listingId == Guid.Empty) throw new ArgumentException("ListingId required");
            if (createdBy == Guid.Empty) throw new ArgumentException("CreatedBy required");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title required");

            return new ShootOrder
            {
                AgencyId = agencyId,
                StudioId = studioId,
                ListingId = listingId,
                CreatedBy = createdBy,
                Title = title.Trim(),
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
            // Tasks can be added after acceptance, so we don't enforce this check here
            Status = ShootOrderStatus.Accepted; Touch();
        }

        public void MarkScheduled()
        {
            if (Status != ShootOrderStatus.Accepted && Status != ShootOrderStatus.Scheduled)
                throw new InvalidOperationException("Must be accepted first");
            // Require at least one task before scheduling
            if (_tasks.Count == 0) throw new InvalidOperationException("Cannot schedule order without tasks");
            Status = ShootOrderStatus.Scheduled; Touch();
        }

        public void Start()
        {
            // Allow starting from both Accepted and Scheduled status
            if (Status != ShootOrderStatus.Scheduled && Status != ShootOrderStatus.Accepted) 
                throw new InvalidOperationException("Order must be accepted or scheduled before starting");
            // Tasks must exist before starting work
            if (_tasks.Count == 0) throw new InvalidOperationException("Cannot start order without tasks");
            Status = ShootOrderStatus.InProgress; Touch();
        }

        public void Complete()
        {
            if (Status != ShootOrderStatus.InProgress && Status != ShootOrderStatus.Scheduled)
                throw new InvalidOperationException("Must be in progress/scheduled");
            Status = ShootOrderStatus.Completed; Touch();
        }

        public void Cancel(string? reason = null)
        {
            if (Status == ShootOrderStatus.Completed) throw new InvalidOperationException("Cannot cancel completed");
            if (Status == ShootOrderStatus.Cancelled) return;
            
            Status = ShootOrderStatus.Cancelled;
            CancellationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            Touch();
        }

        // Photographer Assignment
        public void AssignPhotographer(Guid photographerId)
        {
            if (photographerId == Guid.Empty) throw new ArgumentException("PhotographerId required", nameof(photographerId));
            EnsureNotFinal();
            AssignedPhotographerId = photographerId;
            Touch();
        }

        public void UnassignPhotographer()
        {
            EnsureNotFinal();
            AssignedPhotographerId = null;
            Touch();
        }

        // Scheduling
        public void SetSchedule(DateTime scheduledStartUtc, DateTime? scheduledEndUtc = null, string? notes = null)
        {
            if (scheduledStartUtc < DateTime.UtcNow.AddMinutes(-5)) // Allow 5 min buffer
                throw new ArgumentException("Cannot schedule in the past", nameof(scheduledStartUtc));
            
            if (scheduledEndUtc.HasValue && scheduledEndUtc.Value <= scheduledStartUtc)
                throw new ArgumentException("End time must be after start time", nameof(scheduledEndUtc));

            EnsureNotFinal();
            ScheduledStartUtc = scheduledStartUtc;
            ScheduledEndUtc = scheduledEndUtc;
            SchedulingNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        public void ClearSchedule()
        {
            EnsureNotFinal();
            ScheduledStartUtc = null;
            ScheduledEndUtc = null;
            SchedulingNotes = null;
            Touch();
        }

        private void RecalculateTotal() => TotalAmount = _tasks.Sum(t => t.Price ?? 0m);

        private void EnsureNotFinal()
        {
            if (Status is ShootOrderStatus.Completed or ShootOrderStatus.Cancelled)
                throw new InvalidOperationException("Order is final");
        }
    }
}