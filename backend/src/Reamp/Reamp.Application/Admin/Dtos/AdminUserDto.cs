using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Admin.Dtos
{
    public sealed class AdminUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
