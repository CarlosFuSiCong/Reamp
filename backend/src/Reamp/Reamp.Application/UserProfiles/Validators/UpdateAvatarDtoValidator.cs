using FluentValidation;
using Reamp.Application.UserProfiles.Dtos;

namespace Reamp.Application.UserProfiles.Validators
{
    public sealed class UpdateAvatarDtoValidator : AbstractValidator<UpdateAvatarDto>
    {
        public UpdateAvatarDtoValidator()
        {
            RuleFor(x => x.AvatarAssetId)
                .NotEmpty().WithMessage("AvatarAssetId is required.")
                .When(x => x.AvatarAssetId.HasValue);
        }
    }
}

