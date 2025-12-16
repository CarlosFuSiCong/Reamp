using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Authentication.Dtos
{
    // User information response DTO
    public sealed class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public UserRole Role { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        // Agency information (if user is an agent)
        public Guid? AgencyId { get; set; }
        public int? AgencyRole { get; set; }
        
        // Studio information (if user is a studio staff)
        public Guid? StudioId { get; set; }
        public int? StudioRole { get; set; }
    }
}

