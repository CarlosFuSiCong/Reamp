using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations;

namespace Reamp.Application.Applications.Dtos
{
    public sealed class SubmitAgencyApplicationDto
    {
        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string OrganizationName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact phone is required")]
        [Phone]
        public string ContactPhone { get; set; } = string.Empty;
    }

    public sealed class SubmitStudioApplicationDto
    {
        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string OrganizationName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact phone is required")]
        [Phone]
        public string ContactPhone { get; set; } = string.Empty;

        public AddressDto? Address { get; set; }
    }

    public sealed class ReviewApplicationDto
    {
        [Required]
        public bool Approved { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public sealed class ApplicationListDto
    {
        public Guid Id { get; set; }
        public ApplicationType Type { get; set; }
        public ApplicationStatus Status { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public Guid ApplicantUserId { get; set; }
        public string ApplicantEmail { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ReviewedAtUtc { get; set; }
    }

    public sealed class ApplicationDetailDto
    {
        public Guid Id { get; set; }
        public ApplicationType Type { get; set; }
        public ApplicationStatus Status { get; set; }
        
        public Guid ApplicantUserId { get; set; }
        public string ApplicantEmail { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        
        public string OrganizationName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public AddressDto? Address { get; set; }
        
        public Guid? CreatedOrganizationId { get; set; }
        public Guid? ReviewedBy { get; set; }
        public string? ReviewerName { get; set; }
        public DateTime? ReviewedAtUtc { get; set; }
        public string? ReviewNotes { get; set; }
        
        public DateTime CreatedAtUtc { get; set; }
    }

    public sealed class AddressDto
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public Address? ToValueObject()
        {
            if (string.IsNullOrWhiteSpace(Street) && 
                string.IsNullOrWhiteSpace(City) && 
                string.IsNullOrWhiteSpace(State))
                return null;

            return Address.Create(Street, City, State, PostalCode, Country);
        }
    }
}
