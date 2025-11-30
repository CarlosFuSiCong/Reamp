namespace Reamp.Application.Accounts.Clients.Dtos
{
    public sealed class CreateClientDto
    {
        public Guid UserProfileId { get; set; }
        public Guid AgencyId { get; set; }
        public Guid? AgencyBranchId { get; set; }
    }
}

