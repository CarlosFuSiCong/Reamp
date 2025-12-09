using FluentValidation;

namespace Reamp.Application.Media.Dtos
{
    public class UploadMediaDtoValidator : AbstractValidator<UploadMediaDto>
    {
        public UploadMediaDtoValidator()
        {
            RuleFor(x => x.OwnerStudioId)
                .NotEmpty().WithMessage("Studio ID is required");

            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("File stream is required");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required");

            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("File size must be greater than 0");

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
            });
        }
    }
}

