using Reamp.Domain.Common.Entities;
using Reamp.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Studio : AuditableEntity
    {
        public string Name { get; private set; } = default!;
        public Slug Slug { get; private set; } = default!;
        public string? Description { get; private set; }

        public Guid? LogoAssetId { get; private set; }

        public Guid CreatedBy { get; private set; }

        public string ContactEmail { get; private set; } = default!;
        public string ContactPhone { get; private set; } = default!;

        public Address? Address { get; private set; }

        private Studio() { }

        public static Studio Create(
            string name,
            Guid createdBy,
            string contactEmail,
            string contactPhone,
            string? description = null,
            Guid? logoAssetId = null,
            Address? address = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Studio name is required.", nameof(name));
            if (createdBy == Guid.Empty) throw new ArgumentException("CreatedBy is required.", nameof(createdBy));
            if (string.IsNullOrWhiteSpace(contactEmail)) throw new ArgumentException("Contact email is required.", nameof(contactEmail));
            if (string.IsNullOrWhiteSpace(contactPhone)) throw new ArgumentException("Contact phone is required.", nameof(contactPhone));

            return new Studio
            {
                Name = name.Trim(),
                Slug = Slug.From(name),
                Description = description,
                LogoAssetId = logoAssetId,
                CreatedBy = createdBy,
                ContactEmail = contactEmail.Trim(),
                ContactPhone = contactPhone.Trim(),
                Address = address
            };
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Studio name is required.", nameof(name));
            Name = name.Trim();
            Slug = Slug.From(name);
            Touch();
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
            Touch();
        }

        public void SetLogoAsset(Guid mediaAssetId)
        {
            if (mediaAssetId == Guid.Empty) throw new ArgumentException("MediaAssetId is required.", nameof(mediaAssetId));
            LogoAssetId = mediaAssetId;
            Touch();
        }

        public void ClearLogo()
        {
            LogoAssetId = null;
            Touch();
        }

        public void UpdateContact(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Contact email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Contact phone is required.", nameof(phone));
            ContactEmail = email.Trim();
            ContactPhone = phone.Trim();
            Touch();
        }

        public void UpdateAddress(Address? address)
        {
            Address = address;
            Touch();
        }
    }
}