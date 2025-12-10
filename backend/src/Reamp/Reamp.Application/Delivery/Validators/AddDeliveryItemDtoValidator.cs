using FluentValidation;
using Reamp.Application.Delivery.Dtos;

namespace Reamp.Application.Delivery.Validators
{
    public sealed class AddDeliveryItemDtoValidator : AbstractValidator<AddDeliveryItemDto>
    {
        public AddDeliveryItemDtoValidator()
        {
            RuleFor(x => x.MediaAssetId)
                .NotEmpty()
                .WithMessage("MediaAssetId is required");

            RuleFor(x => x.VariantName)
                .NotEmpty()
                .WithMessage("VariantName is required")
                .MaximumLength(50)
                .WithMessage("VariantName must not exceed 50 characters");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("SortOrder must be non-negative");
        }
    }
}

