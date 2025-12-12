using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Entities;
using System;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Invitation : AuditableEntity
    {
        public InvitationType Type { get; private set; }
        public Guid TargetEntityId { get; private set; }
        public Guid? TargetBranchId { get; private set; }

        public string InviteeEmail { get; private set; } = string.Empty;
        public Guid? InviteeUserId { get; private set; }

        public int TargetRoleValue { get; private set; }
        
        public InvitationStatus Status { get; private set; }
        
        public Guid InvitedBy { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public DateTime? RespondedAtUtc { get; private set; }

        private Invitation() { }

        public static Invitation CreateAgencyInvitation(
            Guid agencyId,
            string inviteeEmail,
            AgencyRole targetRole,
            Guid invitedBy,
            Guid? agencyBranchId = null,
            int expirationDays = 7)
        {
            if (agencyId == Guid.Empty)
                throw new ArgumentException("AgencyId is required.", nameof(agencyId));
            if (string.IsNullOrWhiteSpace(inviteeEmail))
                throw new ArgumentException("Invitee email is required.", nameof(inviteeEmail));
            if (invitedBy == Guid.Empty)
                throw new ArgumentException("InvitedBy is required.", nameof(invitedBy));
            if (expirationDays <= 0)
                throw new ArgumentException("Expiration days must be positive.", nameof(expirationDays));

            return new Invitation
            {
                Type = InvitationType.Agency,
                TargetEntityId = agencyId,
                TargetBranchId = agencyBranchId,
                InviteeEmail = inviteeEmail.Trim().ToLowerInvariant(),
                TargetRoleValue = (int)targetRole,
                Status = InvitationStatus.Pending,
                InvitedBy = invitedBy,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays)
            };
        }

        public static Invitation CreateStudioInvitation(
            Guid studioId,
            string inviteeEmail,
            StudioRole targetRole,
            Guid invitedBy,
            int expirationDays = 7)
        {
            if (studioId == Guid.Empty)
                throw new ArgumentException("StudioId is required.", nameof(studioId));
            if (string.IsNullOrWhiteSpace(inviteeEmail))
                throw new ArgumentException("Invitee email is required.", nameof(inviteeEmail));
            if (invitedBy == Guid.Empty)
                throw new ArgumentException("InvitedBy is required.", nameof(invitedBy));
            if (expirationDays <= 0)
                throw new ArgumentException("Expiration days must be positive.", nameof(expirationDays));

            return new Invitation
            {
                Type = InvitationType.Studio,
                TargetEntityId = studioId,
                InviteeEmail = inviteeEmail.Trim().ToLowerInvariant(),
                TargetRoleValue = (int)targetRole,
                Status = InvitationStatus.Pending,
                InvitedBy = invitedBy,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays)
            };
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAtUtc;

        public bool CanBeAccepted() => 
            Status == InvitationStatus.Pending && !IsExpired();

        public bool CanBeRejected() => 
            Status == InvitationStatus.Pending && !IsExpired();

        public bool CanBeCancelled() => 
            Status == InvitationStatus.Pending;

        public void Accept(Guid userId)
        {
            if (!CanBeAccepted())
                throw new InvalidOperationException("Invitation cannot be accepted.");

            Status = InvitationStatus.Accepted;
            InviteeUserId = userId;
            RespondedAtUtc = DateTime.UtcNow;
            Touch();
        }

        public void Reject()
        {
            if (!CanBeRejected())
                throw new InvalidOperationException("Invitation cannot be rejected.");

            Status = InvitationStatus.Rejected;
            RespondedAtUtc = DateTime.UtcNow;
            Touch();
        }

        public void Cancel()
        {
            if (!CanBeCancelled())
                throw new InvalidOperationException("Invitation cannot be cancelled.");

            Status = InvitationStatus.Cancelled;
            Touch();
        }

        public void MarkAsExpired()
        {
            if (Status == InvitationStatus.Pending && IsExpired())
            {
                Status = InvitationStatus.Expired;
                Touch();
            }
        }

        public AgencyRole GetAgencyRole()
        {
            if (Type != InvitationType.Agency)
                throw new InvalidOperationException("This invitation is not for an agency.");
            
            return (AgencyRole)TargetRoleValue;
        }

        public StudioRole GetStudioRole()
        {
            if (Type != InvitationType.Studio)
                throw new InvalidOperationException("This invitation is not for a studio.");
            
            return (StudioRole)TargetRoleValue;
        }
    }
}
