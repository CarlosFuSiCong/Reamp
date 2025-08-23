using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    // User profile (one-to-one with ApplicationUser)
    public sealed class UserProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DisplayName => $"{FirstName} {LastName}".Trim();

        public string? AvatarUrl { get; private set; }

        public UserRole Role { get; private set; }
        public UserStatus Status { get; private set; }

        // For ORM
        private UserProfile() { }

        // Create new profile
        public static UserProfile Create(Guid applicationUserId, string firstName, string lastName, UserRole role = UserRole.User, string? avatarUrl = null)
        {
            if (applicationUserId == Guid.Empty)
                throw new ArgumentException("ApplicationUserId is required.", nameof(applicationUserId));

            var profile = new UserProfile
            {
                ApplicationUserId = applicationUserId,
                Role = role,
                Status = UserStatus.Active
            };

            profile.SetName(firstName, lastName);
            profile.SetAvatar(avatarUrl);
            return profile;
        }

        public void UpdateBasicInfo(string firstName, string lastName, string? avatarUrl)
        {
            EnsureNotDeleted();
            SetName(firstName, lastName);
            SetAvatar(avatarUrl);
            Touch();
        }

        public void SetRole(UserRole role)
        {
            EnsureNotDeleted();
            Role = role;
            Touch();
        }

        public void Activate()
        {
            EnsureNotDeleted();
            if (Status != UserStatus.Active)
            {
                Status = UserStatus.Active;
                Touch();
            }
        }

        public void Deactivate()
        {
            EnsureNotDeleted();
            if (Status != UserStatus.Inactive)
            {
                Status = UserStatus.Inactive;
                Touch();
            }
        }

        // ==== Internal helpers ====
        private void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName.Length > 40)
                throw new ArgumentException("First name max length 40", nameof(firstName));
            if (lastName.Length > 40)
                throw new ArgumentException("Last name max length 40", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
        }

        private void SetAvatar(string? avatarUrl)
        {
            AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Profile is deleted");
        }
    }
}
