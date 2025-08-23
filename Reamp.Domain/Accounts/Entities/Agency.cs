using Reamp.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reamp.Domain.Common.Entities;

namespace Reamp.Domain.Accounts.Entities
{
    public sealed class Agency : AuditableEntity
    {
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public string? Description { get; private set; }

        public string? LogoUrl { get; private set; }
        public Guid CreatedBy { get; private set; }

        public string ContactEmail { get; private set; }
        public string ContactPhone { get; private set; }

        private Agency() { }

        public static Agency Create(
            string name,
            Guid createdBy,
            string contactEmail,
            string contactPhone,
            string? description = null,
            string? logoUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Agency name cannot be empty.");
            if (string.IsNullOrWhiteSpace(contactEmail)) throw new ArgumentException("Contact email is required.");
            if (string.IsNullOrWhiteSpace(contactPhone)) throw new ArgumentException("Contact phone is required.");

            return new Agency
            {
                Name = name.Trim(),
                Slug = ToSlug(name),
                Description = description,
                LogoUrl = logoUrl,
                CreatedBy = createdBy,
                ContactEmail = contactEmail.Trim(),
                ContactPhone = contactPhone.Trim()
            };
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Agency name cannot be empty.");
            Name = name.Trim();
            Touch();
        }

        public void UpdateLogo(string? logoUrl) { LogoUrl = logoUrl; Touch(); }
        public void UpdateDescription(string? description) { Description = description; Touch(); }

        public void UpdateContact(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Contact email is required.");
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Contact phone is required.");
            ContactEmail = email.Trim();
            ContactPhone = phone.Trim();
            Touch();
        }

        private static string ToSlug(string s) =>
            string.Join("-", s.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
