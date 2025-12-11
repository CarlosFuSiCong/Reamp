using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Authentication.Dtos
{
    // Registration DTO for invited users (Agent/Staff)
    // Requires invitation token from admin
    public sealed class InvitedRegisterDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole Role { get; set; }
        public string InvitationToken { get; set; } = default!; // Required for non-User roles
    }
}

