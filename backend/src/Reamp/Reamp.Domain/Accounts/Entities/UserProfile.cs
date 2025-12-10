using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class UserProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; private set; }

        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string DisplayName => $"{FirstName} {LastName}".Trim();

        public Guid? AvatarAssetId { get; private set; }

        public UserRole Role { get; private set; } = UserRole.User;
        public UserStatus Status { get; private set; } = UserStatus.Active;

        private UserProfile() { }

        public static UserProfile Create(Guid applicationUserId, string firstName, string lastName, UserRole role = UserRole.User, Guid? avatarAssetId = null)
        {
            if (applicationUserId == Guid.Empty)
                throw new ArgumentException("ApplicationUserId is required.", nameof(applicationUserId));

            var profile = new UserProfile
            {
                ApplicationUserId = applicationUserId,
                Role = role,
                Status = UserStatus.Active,
                AvatarAssetId = avatarAssetId
            };

            profile.SetName(firstName, lastName);
            return profile;
        }

        public void UpdateBasicInfo(string firstName, string lastName, Guid? avatarAssetId = null)
        {
            EnsureNotDeleted();
            SetName(firstName, lastName);
            AvatarAssetId = avatarAssetId;
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

        private void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName.Length > 40)
                throw new ArgumentException("First name max length is 40 characters.", nameof(firstName));
            if (lastName.Length > 40)
                throw new ArgumentException("Last name max length is 40 characters.", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Profile is deleted");
        }

        public void SetAvatarAsset(Guid mediaAssetId)
        {
            if (mediaAssetId == Guid.Empty) throw new ArgumentException("MediaAssetId is required.", nameof(mediaAssetId));
            AvatarAssetId = mediaAssetId;
            Touch();
        }

        public void ClearAvatar()
        {
            AvatarAssetId = null;
            Touch();
        }
    }
}