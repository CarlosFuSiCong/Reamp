using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Entities;
using Reamp.Domain.Common.ValueObjects;
using System;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class OrganizationApplication : AuditableEntity
    {
        public ApplicationType Type { get; private set; }
        public ApplicationStatus Status { get; private set; }

        public Guid ApplicantUserId { get; private set; }

        public string OrganizationName { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string ContactEmail { get; private set; } = string.Empty;
        public string ContactPhone { get; private set; } = string.Empty;
        public Address? Address { get; private set; }

        public Guid? CreatedOrganizationId { get; private set; }

        public Guid? ReviewedBy { get; private set; }
        public DateTime? ReviewedAtUtc { get; private set; }
        public string? ReviewNotes { get; private set; }

        private OrganizationApplication() { }

        public static OrganizationApplication CreateAgencyApplication(
            Guid applicantUserId,
            string organizationName,
            string contactEmail,
            string contactPhone,
            string? description = null)
        {
            ValidateInputs(applicantUserId, organizationName, contactEmail, contactPhone);

            return new OrganizationApplication
            {
                Type = ApplicationType.Agency,
                Status = ApplicationStatus.Pending,
                ApplicantUserId = applicantUserId,
                OrganizationName = organizationName.Trim(),
                Description = description,
                ContactEmail = contactEmail.Trim(),
                ContactPhone = contactPhone.Trim()
            };
        }

        public static OrganizationApplication CreateStudioApplication(
            Guid applicantUserId,
            string organizationName,
            string contactEmail,
            string contactPhone,
            string? description = null,
            Address? address = null)
        {
            ValidateInputs(applicantUserId, organizationName, contactEmail, contactPhone);

            return new OrganizationApplication
            {
                Type = ApplicationType.Studio,
                Status = ApplicationStatus.Pending,
                ApplicantUserId = applicantUserId,
                OrganizationName = organizationName.Trim(),
                Description = description,
                ContactEmail = contactEmail.Trim(),
                ContactPhone = contactPhone.Trim(),
                Address = address
            };
        }

        private static void ValidateInputs(Guid applicantUserId, string organizationName, string contactEmail, string contactPhone)
        {
            if (applicantUserId == Guid.Empty)
                throw new ArgumentException("ApplicantUserId is required.", nameof(applicantUserId));
            if (string.IsNullOrWhiteSpace(organizationName))
                throw new ArgumentException("Organization name is required.", nameof(organizationName));
            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new ArgumentException("Contact email is required.", nameof(contactEmail));
            if (string.IsNullOrWhiteSpace(contactPhone))
                throw new ArgumentException("Contact phone is required.", nameof(contactPhone));
        }

        public void MarkAsUnderReview()
        {
            if (Status != ApplicationStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be marked as under review.");

            Status = ApplicationStatus.UnderReview;
            Touch();
        }

        public void Approve(Guid reviewedBy, Guid createdOrganizationId, string? notes = null)
        {
            if (Status != ApplicationStatus.Pending && Status != ApplicationStatus.UnderReview)
                throw new InvalidOperationException("Only pending or under review applications can be approved.");
            if (reviewedBy == Guid.Empty)
                throw new ArgumentException("ReviewedBy is required.", nameof(reviewedBy));
            if (createdOrganizationId == Guid.Empty)
                throw new ArgumentException("CreatedOrganizationId is required.", nameof(createdOrganizationId));

            Status = ApplicationStatus.Approved;
            ReviewedBy = reviewedBy;
            ReviewedAtUtc = DateTime.UtcNow;
            ReviewNotes = notes;
            CreatedOrganizationId = createdOrganizationId;
            Touch();
        }

        public void Reject(Guid reviewedBy, string? notes = null)
        {
            if (Status != ApplicationStatus.Pending && Status != ApplicationStatus.UnderReview)
                throw new InvalidOperationException("Only pending or under review applications can be rejected.");
            if (reviewedBy == Guid.Empty)
                throw new ArgumentException("ReviewedBy is required.", nameof(reviewedBy));

            Status = ApplicationStatus.Rejected;
            ReviewedBy = reviewedBy;
            ReviewedAtUtc = DateTime.UtcNow;
            ReviewNotes = notes;
            Touch();
        }

        public void Cancel()
        {
            if (Status != ApplicationStatus.Pending && Status != ApplicationStatus.UnderReview)
                throw new InvalidOperationException("Only pending or under review applications can be cancelled.");

            Status = ApplicationStatus.Cancelled;
            Touch();
        }

        public bool CanBeReviewed() => 
            Status == ApplicationStatus.Pending || Status == ApplicationStatus.UnderReview;

        public bool CanBeCancelled() => 
            Status == ApplicationStatus.Pending || Status == ApplicationStatus.UnderReview;
    }
}
