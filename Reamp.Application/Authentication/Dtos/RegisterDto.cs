using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Authentication.Dtos
{
    // User registration request DTO
    public sealed class RegisterDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.User;
    }
}

