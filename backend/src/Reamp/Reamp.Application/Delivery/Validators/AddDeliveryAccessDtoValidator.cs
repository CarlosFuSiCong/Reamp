using FluentValidation;
using Reamp.Application.Delivery.Dtos;
using Reamp.Domain.Delivery.Enums;

namespace Reamp.Application.Delivery.Validators
{
    public sealed class AddDeliveryAccessDtoValidator : AbstractValidator<AddDeliveryAccessDto>
    {
        public AddDeliveryAccessDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid access type");

            RuleFor(x => x.RecipientEmail)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.RecipientEmail))
                .WithMessage("Invalid email address");

            RuleFor(x => x.MaxDownloads)
                .GreaterThan(0)
                .When(x => x.MaxDownloads.HasValue)
                .WithMessage("MaxDownloads must be greater than 0");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("Password must be at least 6 characters");
        }
    }
}

