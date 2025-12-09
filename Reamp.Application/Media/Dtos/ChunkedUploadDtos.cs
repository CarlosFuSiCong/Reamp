namespace Reamp.Application.Media.Dtos
{
    // DTO for initiating chunked upload
    public class InitiateChunkedUploadDto
    {
        public Guid OwnerStudioId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public int TotalChunks { get; set; }
        public string? Description { get; set; }
    }

    // DTO for uploading a chunk
    public class UploadChunkDto
    {
        public Guid UploadSessionId { get; set; }
        public int ChunkIndex { get; set; }
        public Stream ChunkData { get; set; } = null!;
    }

    // DTO for upload session info
    public class UploadSessionDto
    {
        public Guid SessionId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public int TotalChunks { get; set; }
        public int UploadedChunks { get; set; }
        public double Progress { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
    }
}



