using FluentValidation;
using Reamp.Application.Authentication.Dtos;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Authentication.Validators
{
    // Validator for RegisterDto
    public sealed class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(120).WithMessage("Email max length is 120");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password max length is 100");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.FirstName)
                .MaximumLength(40).WithMessage("First name max length is 40")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(40).WithMessage("Last name max length is 40")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Role)
                .Equal(UserRole.Client).WithMessage("Public registration is only available for Client users. Please contact an administrator for other account types.");
        }
    }
}

