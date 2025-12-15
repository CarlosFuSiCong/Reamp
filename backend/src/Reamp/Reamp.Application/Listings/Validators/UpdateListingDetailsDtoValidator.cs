using FluentValidation;
using Reamp.Application.Listings.Dtos;

namespace Reamp.Application.Listings.Validators
{
    public sealed class UpdateListingDetailsDtoValidator : AbstractValidator<UpdateListingDetailsDto>
    {
        public UpdateListingDetailsDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

            RuleFor(x => x.Currency)
                .Length(3).WithMessage("Currency must be a 3-character ISO code.")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be 3 uppercase letters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Currency));

            RuleFor(x => x.Address)
                .NotNull().WithMessage("Address is required.");

            RuleFor(x => x.Bedrooms)
                .GreaterThanOrEqualTo(0).WithMessage("Bedrooms must be non-negative.")
                .When(x => x.Bedrooms.HasValue);

            RuleFor(x => x.Bathrooms)
                .GreaterThanOrEqualTo(0).WithMessage("Bathrooms must be non-negative.")
                .When(x => x.Bathrooms.HasValue);

            RuleFor(x => x.ParkingSpaces)
                .GreaterThanOrEqualTo(0).WithMessage("ParkingSpaces must be non-negative.")
                .When(x => x.ParkingSpaces.HasValue);

            RuleFor(x => x.FloorAreaSqm)
                .GreaterThan(0).WithMessage("FloorAreaSqm must be positive.")
                .When(x => x.FloorAreaSqm.HasValue);

            RuleFor(x => x.LandAreaSqm)
                .GreaterThan(0).WithMessage("LandAreaSqm must be positive.")
                .When(x => x.LandAreaSqm.HasValue);
        }
    }
}

