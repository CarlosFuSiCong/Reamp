using Reamp.Domain.Common.Entities;
using Reamp.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Agency : AuditableEntity
    {
        public string Name { get; private set; }
        public Slug Slug { get; private set; }
        public string? Description { get; private set; }

        public Guid? LogoAssetId { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string ContactEmail { get; private set; }
        public string ContactPhone { get; private set; }

        private readonly List<AgencyBranch> _branches = new();
        public IReadOnlyCollection<AgencyBranch> Branches => _branches.AsReadOnly();

        private Agency() { }

        public static Agency Create(
            string name,
            Guid createdBy,
            string contactEmail,
            string contactPhone,
            string? description = null,
            Guid? logoAssetId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Agency name is required.", nameof(name));
            if (createdBy == Guid.Empty) throw new ArgumentException("CreatedBy is required.", nameof(createdBy));
            if (string.IsNullOrWhiteSpace(contactEmail)) throw new ArgumentException("Contact email is required.", nameof(contactEmail));
            if (string.IsNullOrWhiteSpace(contactPhone)) throw new ArgumentException("Contact phone is required.", nameof(contactPhone));

            return new Agency
            {
                Name = name.Trim(),
                Slug = Slug.From(name),
                Description = description,
                LogoAssetId = logoAssetId,
                CreatedBy = createdBy,
                ContactEmail = contactEmail.Trim(),
                ContactPhone = contactPhone.Trim()
            };
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Agency name is required.", nameof(name));
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

        public AgencyBranch AddBranch(
            string name,
            Guid createdBy,
            string contactEmail,
            string contactPhone,
            string? description = null,
            Address? address = null)
        {
            if (Id == Guid.Empty)
                throw new InvalidOperationException("Persist the Agency before adding branches.");

            var branch = AgencyBranch.Create(
                agencyId: Id,
                name: name,
                createdBy: createdBy,
                contactEmail: contactEmail,
                contactPhone: contactPhone,
                description: description,
                address: address
            );

            _branches.Add(branch);
            Touch();
            return branch;
        }
    }
}