namespace Reamp.Application.Authentication.Dtos
{
    // Change password request DTO
    public sealed class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}




