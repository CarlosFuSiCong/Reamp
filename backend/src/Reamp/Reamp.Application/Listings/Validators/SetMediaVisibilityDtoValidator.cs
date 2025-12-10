using FluentValidation;
using Reamp.Application.Listings.Dtos;

namespace Reamp.Application.Listings.Validators
{
    public sealed class SetMediaVisibilityDtoValidator : AbstractValidator<SetMediaVisibilityDto>
    {
        public SetMediaVisibilityDtoValidator()
        {
            RuleFor(x => x.MediaId)
                .NotEmpty().WithMessage("MediaId is required.");
        }
    }
}
