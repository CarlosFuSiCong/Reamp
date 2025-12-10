using FluentValidation;
using Reamp.Application.Listings.Dtos;

namespace Reamp.Application.Listings.Validators
{
    public sealed class AddMediaDtoValidator : AbstractValidator<AddMediaDto>
    {
        public AddMediaDtoValidator()
        {
            RuleFor(x => x.MediaAssetId)
                .NotEmpty().WithMessage("MediaAssetId is required.");
        }
    }
}

