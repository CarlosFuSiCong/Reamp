namespace Reamp.Application.Accounts.Clients.Dtos
{
    public sealed class ClientListDto
    {
        public Guid Id { get; set; }
        public Guid UserProfileId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public Guid AgencyId { get; set; }
        public string? AgencyName { get; set; }
        public Guid? AgencyBranchId { get; set; }
        public string? AgencyBranchName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

