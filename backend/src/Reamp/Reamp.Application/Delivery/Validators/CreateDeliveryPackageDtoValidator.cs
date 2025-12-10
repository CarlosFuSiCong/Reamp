using FluentValidation;
using Reamp.Application.Delivery.Dtos;

namespace Reamp.Application.Delivery.Validators
{
    public sealed class CreateDeliveryPackageDtoValidator : AbstractValidator<CreateDeliveryPackageDto>
    {
        public CreateDeliveryPackageDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("OrderId is required");

            RuleFor(x => x.ListingId)
                .NotEmpty()
                .WithMessage("ListingId is required");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(160)
                .WithMessage("Title must not exceed 160 characters");

            RuleFor(x => x.ExpiresAtUtc)
                .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
                .WithMessage("Expiration date must be in the future");
        }
    }
}

