using Reamp.Domain.Delivery.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Delivery.Entities
{
    public sealed class DeliveryAccess
    {
        public Guid Id { get; private set; }
        public Guid DeliveryPackageId { get; private set; }
        public AccessType Type { get; private set; } = AccessType.Public;

        public string? RecipientEmail { get; private set; }  // for Private/Token
        public string? RecipientName { get; private set; }
        public int? MaxDownloads { get; private set; }  // per recipient or per token
        public string? PasswordHash { get; private set; }  // if password-protected

        public int Downloads { get; private set; }           // simple counter

        private DeliveryAccess() { }

        internal static DeliveryAccess Create(Guid packageId, AccessType type, string? email, string? name, int? maxDownloads, string? passwordHash)
        {
            if (packageId == Guid.Empty) throw new ArgumentException("PackageId required");

            return new DeliveryAccess
            {
                Id = Guid.NewGuid(),
                DeliveryPackageId = packageId,
                Type = type,
                RecipientEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                RecipientName = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
                MaxDownloads = maxDownloads is <= 0 ? null : maxDownloads,
                PasswordHash = string.IsNullOrWhiteSpace(passwordHash) ? null : passwordHash.Trim()
            };
        }

        public void IncrementDownloads()
        {
            Downloads++;
        }
    }
}