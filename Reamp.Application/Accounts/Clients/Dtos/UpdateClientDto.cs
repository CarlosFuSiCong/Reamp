namespace Reamp.Application.Accounts.Clients.Dtos
{
    public sealed class UpdateClientDto
    {
        public Guid AgencyId { get; set; }
        public Guid? AgencyBranchId { get; set; }
    }
}



