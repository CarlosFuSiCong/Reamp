using FluentValidation;
using Reamp.Application.Orders.Dtos;

namespace Reamp.Application.Orders.Validators
{
    public sealed class PlaceOrderDtoValidator : AbstractValidator<PlaceOrderDto>
    {
        public PlaceOrderDtoValidator()
        {
            RuleFor(x => x.AgencyId)
                .NotEmpty().WithMessage("AgencyId is required.");

            RuleFor(x => x.StudioId)
                .NotEmpty().WithMessage("StudioId is required.");

            RuleFor(x => x.ListingId)
                .NotEmpty().WithMessage("ListingId is required.");

            RuleFor(x => x.Currency)
                .Length(3).WithMessage("Currency must be a 3-character ISO code (e.g., AUD, USD, EUR).")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be 3 uppercase letters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Currency));
        }
    }
}



