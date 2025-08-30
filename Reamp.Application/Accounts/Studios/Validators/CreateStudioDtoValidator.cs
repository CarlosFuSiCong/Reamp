using FluentValidation;
using Reamp.Application.Accounts.Studios.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Accounts.Studios.Validators
{
    public sealed class CreateStudioDtoValidator : AbstractValidator<CreateStudioDto>
    {
        public CreateStudioDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120);

            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(120);

            RuleFor(x => x.ContactPhone)
                .NotEmpty()
                .MaximumLength(40);

            RuleFor(x => x.Description)
                .MaximumLength(512)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.LogoUrl)
                .MaximumLength(256)
                .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));

            RuleFor(x => x.Address.Line1)
                .NotEmpty()
                .MaximumLength(120);

            RuleFor(x => x.Address.City)
                .NotEmpty()
                .MaximumLength(80);

            RuleFor(x => x.Address.State)
                .NotEmpty()
                .MaximumLength(40);

            RuleFor(x => x.Address.Postcode)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(x => x.Address.Country)
                .NotEmpty()
                .Length(2);

            RuleFor(x => x.Address.Latitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.Address.Latitude.HasValue);

            RuleFor(x => x.Address.Longitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.Address.Longitude.HasValue);
        }
    }
}