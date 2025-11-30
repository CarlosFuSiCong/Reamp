using FluentValidation;
using Reamp.Application.Accounts.Agencies.Dtos;

namespace Reamp.Application.Accounts.Agencies.Validators
{
    public sealed class CreateAgencyBranchDtoValidator : AbstractValidator<CreateAgencyBranchDto>
    {
        public CreateAgencyBranchDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(120).WithMessage("Branch name cannot exceed 120 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(512).WithMessage("Description cannot exceed 512 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(120).WithMessage("Contact email cannot exceed 120 characters.");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required.")
                .MaximumLength(40).WithMessage("Contact phone cannot exceed 40 characters.");

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address!.Line1)
                    .NotEmpty().WithMessage("Address line 1 is required.")
                    .MaximumLength(120).WithMessage("Address line 1 cannot exceed 120 characters.");

                RuleFor(x => x.Address!.Line2)
                    .MaximumLength(120).WithMessage("Address line 2 cannot exceed 120 characters.")
                    .When(x => !string.IsNullOrWhiteSpace(x.Address!.Line2));

                RuleFor(x => x.Address!.City)
                    .NotEmpty().WithMessage("City is required.")
                    .MaximumLength(80).WithMessage("City cannot exceed 80 characters.");

                RuleFor(x => x.Address!.State)
                    .NotEmpty().WithMessage("State is required.")
                    .MaximumLength(40).WithMessage("State cannot exceed 40 characters.");

                RuleFor(x => x.Address!.Postcode)
                    .NotEmpty().WithMessage("Postcode is required.")
                    .MaximumLength(10).WithMessage("Postcode cannot exceed 10 characters.");

                RuleFor(x => x.Address!.Country)
                    .NotEmpty().WithMessage("Country is required.")
                    .Length(2).WithMessage("Country must be 2-letter ISO code (e.g., AU).");

                RuleFor(x => x.Address!.Latitude)
                    .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
                    .When(x => x.Address!.Latitude.HasValue);

                RuleFor(x => x.Address!.Longitude)
                    .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
                    .When(x => x.Address!.Longitude.HasValue);
            });
        }
    }
}

