namespace Reamp.Application.Media.Dtos
{
    // DTO for uploading media (file stream provided by controller)
    public class UploadMediaDto
    {
        public Guid OwnerStudioId { get; set; }
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
    }
}

