using FluentValidation;

namespace Reamp.Application.Media.Dtos
{
    public class AddVariantDtoValidator : AbstractValidator<AddVariantDto>
    {
        public AddVariantDtoValidator()
        {
            RuleFor(x => x.VariantName)
                .NotEmpty().WithMessage("Variant name is required")
                .MaximumLength(100).WithMessage("Variant name cannot exceed 100 characters");

            When(x => x.Width.HasValue, () =>
            {
                RuleFor(x => x.Width)
                    .GreaterThan(0).WithMessage("Width must be greater than 0")
                    .LessThanOrEqualTo(10000).WithMessage("Width cannot exceed 10000 pixels");
            });

            When(x => x.Height.HasValue, () =>
            {
                RuleFor(x => x.Height)
                    .GreaterThan(0).WithMessage("Height must be greater than 0")
                    .LessThanOrEqualTo(10000).WithMessage("Height cannot exceed 10000 pixels");
            });
        }
    }
}



