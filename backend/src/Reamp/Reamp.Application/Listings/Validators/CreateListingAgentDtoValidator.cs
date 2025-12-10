using FluentValidation;
using Reamp.Application.Listings.Dtos;

namespace Reamp.Application.Listings.Validators
{
    public sealed class CreateListingAgentDtoValidator : AbstractValidator<CreateListingAgentDto>
    {
        public CreateListingAgentDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(100).WithMessage("FirstName cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(100).WithMessage("LastName cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Team)
                .MaximumLength(100).WithMessage("Team cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Team));

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("SortOrder must be non-negative.");
        }
    }
}

