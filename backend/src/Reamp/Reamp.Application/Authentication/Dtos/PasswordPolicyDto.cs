namespace Reamp.Application.Authentication.Dtos
{
    public sealed class PasswordPolicyDto
    {
        public int RequiredLength { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
    }
}

