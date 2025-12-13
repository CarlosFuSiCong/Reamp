using FluentValidation;
using Reamp.Application.UserProfiles.Dtos;

namespace Reamp.Application.UserProfiles.Validators
{
    public sealed class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UpdateUserProfileDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .WithMessage("First name cannot be null");

            RuleFor(x => x.FirstName)
                .MaximumLength(40)
                .WithMessage("First name must not exceed 40 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .NotNull()
                .WithMessage("Last name cannot be null");

            RuleFor(x => x.LastName)
                .MaximumLength(40)
                .WithMessage("Last name must not exceed 40 characters")
                .When(x => !string.IsNullOrEmpty(x.LastName));
        }
    }
}

