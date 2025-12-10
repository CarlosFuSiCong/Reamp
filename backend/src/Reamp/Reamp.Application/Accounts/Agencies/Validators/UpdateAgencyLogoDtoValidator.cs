using FluentValidation;
using Reamp.Application.Accounts.Agencies.Dtos;

namespace Reamp.Application.Accounts.Agencies.Validators
{
    public sealed class UpdateAgencyLogoDtoValidator : AbstractValidator<UpdateAgencyLogoDto>
    {
        public UpdateAgencyLogoDtoValidator()
        {
            RuleFor(x => x.LogoAssetId)
                .NotEmpty().WithMessage("LogoAssetId is required.")
                .When(x => x.LogoAssetId.HasValue);
        }
    }
}

