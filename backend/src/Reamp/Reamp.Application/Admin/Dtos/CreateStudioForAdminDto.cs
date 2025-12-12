namespace Reamp.Application.Admin.Dtos
{
    public sealed class CreateStudioForAdminDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public Guid OwnerUserId { get; set; }
    }
}
