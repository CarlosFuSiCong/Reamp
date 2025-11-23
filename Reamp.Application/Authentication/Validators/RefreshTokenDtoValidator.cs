using FluentValidation;
using Reamp.Application.Authentication.Dtos;

namespace Reamp.Application.Authentication.Validators
{
    // Validator for RefreshTokenDto
    public sealed class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        }
    }
}

