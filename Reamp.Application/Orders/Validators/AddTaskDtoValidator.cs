using FluentValidation;
using Reamp.Application.Orders.Dtos;

namespace Reamp.Application.Orders.Validators
{
    public sealed class AddTaskDtoValidator : AbstractValidator<AddTaskDto>
    {
        public AddTaskDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid task type.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Notes));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .When(x => x.Price.HasValue);
        }
    }
}

