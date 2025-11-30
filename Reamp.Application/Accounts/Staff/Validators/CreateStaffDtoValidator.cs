using FluentValidation;
using Reamp.Application.Accounts.Staff.Dtos;

namespace Reamp.Application.Accounts.Staff.Validators
{
    public sealed class CreateStaffDtoValidator : AbstractValidator<CreateStaffDto>
    {
        public CreateStaffDtoValidator()
        {
            RuleFor(x => x.UserProfileId)
                .NotEmpty().WithMessage("UserProfileId is required.");

            RuleFor(x => x.StudioId)
                .NotEmpty().WithMessage("StudioId is required.");
        }
    }
}

