namespace Reamp.Application.Accounts.Agencies.Dtos
{
    public sealed class CreateAgencyDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid? LogoAssetId { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
    }
}



