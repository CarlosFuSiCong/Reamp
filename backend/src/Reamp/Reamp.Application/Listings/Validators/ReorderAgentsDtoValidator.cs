using FluentValidation;
using Reamp.Application.Listings.Dtos;

namespace Reamp.Application.Listings.Validators
{
    public sealed class ReorderAgentsDtoValidator : AbstractValidator<ReorderAgentsDto>
    {
        public ReorderAgentsDtoValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Items cannot be empty.");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.Id)
                        .NotEmpty().WithMessage("Id is required.");
                    
                    item.RuleFor(x => x.SortOrder)
                        .GreaterThanOrEqualTo(0).WithMessage("SortOrder must be non-negative.");
                });
        }
    }
}

