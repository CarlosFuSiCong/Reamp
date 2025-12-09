using FluentValidation;
using Reamp.Application.Orders.Dtos;

namespace Reamp.Application.Orders.Validators
{
    public sealed class CancelOrderDtoValidator : AbstractValidator<CancelOrderDto>
    {
        public CancelOrderDtoValidator()
        {
            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}



