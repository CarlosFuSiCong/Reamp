using FluentValidation;
using Reamp.Application.Accounts.Clients.Dtos;

namespace Reamp.Application.Accounts.Clients.Validators
{
    public sealed class UpdateClientDtoValidator : AbstractValidator<UpdateClientDto>
    {
        public UpdateClientDtoValidator()
        {
            RuleFor(x => x.AgencyId)
                .NotEmpty().WithMessage("AgencyId is required.");
        }
    }
}

