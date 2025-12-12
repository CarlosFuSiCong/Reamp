using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Admin.Dtos
{
    public sealed class UpdateUserStatusDto
    {
        public UserStatus Status { get; set; }
    }
}
