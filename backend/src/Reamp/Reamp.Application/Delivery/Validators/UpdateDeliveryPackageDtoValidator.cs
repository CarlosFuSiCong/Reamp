using FluentValidation;
using Reamp.Application.Delivery.Dtos;

namespace Reamp.Application.Delivery.Validators
{
    public sealed class UpdateDeliveryPackageDtoValidator : AbstractValidator<UpdateDeliveryPackageDto>
    {
        public UpdateDeliveryPackageDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Title));

            RuleFor(x => x.ExpiresAtUtc)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("ExpiresAtUtc must be in the future.")
                .When(x => x.ExpiresAtUtc.HasValue);
        }
    }
}
