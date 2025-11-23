namespace Reamp.Application.Authentication.Dtos
{
    // User login request DTO
    public sealed class LoginDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}

