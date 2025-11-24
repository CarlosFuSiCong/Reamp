using FluentValidation;
using Reamp.Application.Accounts.Agencies.Dtos;

namespace Reamp.Application.Accounts.Agencies.Validators
{
    public sealed class UpdateAgencyDtoValidator : AbstractValidator<UpdateAgencyDto>
    {
        public UpdateAgencyDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Agency name is required.")
                .MaximumLength(120).WithMessage("Agency name cannot exceed 120 characters.");

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

            RuleFor(x => x.LogoAssetId)
                .NotEqual(Guid.Empty).WithMessage("Invalid LogoAssetId.")
                .When(x => x.LogoAssetId.HasValue);
        }
    }
}

