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
        public string Name { get; private set; }
        public Slug Slug { get; private set; }
        public string? Description { get; private set; }

        public string? LogoUrl { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string ContactEmail { get; private set; }
        public string ContactPhone { get; private set; }

        public Address? Address { get; private set; }

        private Studio() { }

        public static Studio Create(
            string name,
            Guid createdBy,
            string contactEmail,
            string contactPhone,
            string? description = null,
            string? logoUrl = null,
            Address? address = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Studio name is required.");
            if (string.IsNullOrWhiteSpace(contactEmail)) throw new ArgumentException("Contact email is required.");
            if (string.IsNullOrWhiteSpace(contactPhone)) throw new ArgumentException("Contact phone is required.");

            return new Studio
            {
                Name = name.Trim(),
                Slug = Slug.From(name),
                Description = description,
                LogoUrl = logoUrl,
                CreatedBy = createdBy,
                ContactEmail = contactEmail.Trim(),
                ContactPhone = contactPhone.Trim(),
                Address = address
            };
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Studio name is required.");
            Name = name.Trim();
            Slug = Slug.From(name);
            Touch();
        }

        public void UpdateDescription(string? description) { Description = description; Touch(); }
        public void UpdateLogo(string? logoUrl) { LogoUrl = logoUrl; Touch(); }

        public void UpdateContact(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Contact email is required.");
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Contact phone is required.");
            ContactEmail = email.Trim();
            ContactPhone = phone.Trim();
            Touch();
        }

        public void UpdateAddress(Address? address) { Address = address; Touch(); }
    }
}