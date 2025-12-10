using FluentValidation;
using Reamp.Application.Orders.Dtos;

namespace Reamp.Application.Orders.Validators
{
    public sealed class AssignPhotographerDtoValidator : AbstractValidator<AssignPhotographerDto>
    {
        public AssignPhotographerDtoValidator()
        {
            RuleFor(x => x.PhotographerId)
                .NotEmpty().WithMessage("PhotographerId is required.");
        }
    }
}

