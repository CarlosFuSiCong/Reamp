using FluentValidation;
using Reamp.Application.Authentication.Dtos;

namespace Reamp.Application.Authentication.Validators
{
    // Validator for UpdateProfileDto
    public sealed class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(40).WithMessage("First name max length is 40");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(40).WithMessage("Last name max length is 40");
        }
    }
}

