using FluentValidation;
using Reamp.Application.Listings.Dtos;

namespace Reamp.Application.Listings.Validators
{
    public sealed class CreateListingDtoValidator : AbstractValidator<CreateListingDto>
    {
        public CreateListingDtoValidator()
        {
            // OwnerAgencyId is set by the controller from the authenticated user's agent record
            // No validation needed here

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be a 3-character ISO code (e.g., AUD, USD).")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be 3 uppercase letters.");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("Address is required.");
        }
    }
}

