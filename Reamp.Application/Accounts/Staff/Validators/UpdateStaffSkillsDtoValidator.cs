using FluentValidation;
using Reamp.Application.Accounts.Staff.Dtos;

namespace Reamp.Application.Accounts.Staff.Validators
{
    public sealed class UpdateStaffSkillsDtoValidator : AbstractValidator<UpdateStaffSkillsDto>
    {
        public UpdateStaffSkillsDtoValidator()
        {
            // StaffSkills is an enum, no specific validation needed beyond type safety
        }
    }
}

