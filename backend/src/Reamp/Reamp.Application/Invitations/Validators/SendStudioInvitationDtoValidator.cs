using FluentValidation;
using Reamp.Application.Invitations.Dtos;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Invitations.Validators
{
    public sealed class SendStudioInvitationDtoValidator : AbstractValidator<SendStudioInvitationDto>
    {
        public SendStudioInvitationDtoValidator()
        {
            RuleFor(x => x.InviteeEmail)
                .NotEmpty().WithMessage("Invitee email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

            RuleFor(x => x.TargetRole)
                .IsInEnum().WithMessage("Invalid role specified.");
        }
    }
}
