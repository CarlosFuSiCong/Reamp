using FluentValidation;
using Reamp.Application.Orders.Dtos;

namespace Reamp.Application.Orders.Validators
{
    public sealed class SetScheduleDtoValidator : AbstractValidator<SetScheduleDto>
    {
        public SetScheduleDtoValidator()
        {
            RuleFor(x => x.ScheduledStartUtc)
                .NotEmpty().WithMessage("ScheduledStartUtc is required.")
                .Must(date => date > DateTime.UtcNow)
                .WithMessage("ScheduledStartUtc must be in the future.");

            RuleFor(x => x.ScheduledEndUtc)
                .GreaterThan(x => x.ScheduledStartUtc)
                .WithMessage("ScheduledEndUtc must be after ScheduledStartUtc.")
                .When(x => x.ScheduledEndUtc.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}

