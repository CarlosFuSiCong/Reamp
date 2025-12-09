using FluentValidation;
using Reamp.Application.Media.Dtos;

namespace Reamp.Application.Media.Validators
{
    public class InitiateChunkedUploadDtoValidator : AbstractValidator<InitiateChunkedUploadDto>
    {
        public InitiateChunkedUploadDtoValidator()
        {
            RuleFor(x => x.OwnerStudioId)
                .NotEmpty().WithMessage("Studio ID is required");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required")
                .Must(ct => !string.IsNullOrWhiteSpace(ct) && 
                           (ct.ToLowerInvariant().StartsWith("image/") || 
                            ct.ToLowerInvariant().StartsWith("video/")))
                .WithMessage("Content type must be an image or video type");

            RuleFor(x => x.TotalSize)
                .GreaterThan(0).WithMessage("Total size must be greater than 0")
                .LessThanOrEqualTo(int.MaxValue).WithMessage("Total size cannot exceed 2GB");

            RuleFor(x => x.TotalChunks)
                .GreaterThan(0).WithMessage("Total chunks must be greater than 0")
                .LessThanOrEqualTo(10000).WithMessage("Total chunks cannot exceed 10000");

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
            });
        }
    }
}

