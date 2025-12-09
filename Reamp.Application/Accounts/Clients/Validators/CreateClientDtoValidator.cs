using FluentValidation;
using Reamp.Application.Accounts.Clients.Dtos;

namespace Reamp.Application.Accounts.Clients.Validators
{
    public sealed class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
    {
        public CreateClientDtoValidator()
        {
            RuleFor(x => x.UserProfileId)
                .NotEmpty().WithMessage("UserProfileId is required.");

            RuleFor(x => x.AgencyId)
                .NotEmpty().WithMessage("AgencyId is required.");
        }
    }
}



