using Reamp.Application.Media.Dtos;

namespace Reamp.Application.Media.Services
{
    public interface IChunkedUploadService
    {
        // Initiate a chunked upload session
        Task<UploadSessionDto> InitiateUploadAsync(
            InitiateChunkedUploadDto dto,
            Guid currentUserId,
            CancellationToken ct = default);

        // Upload a chunk
        Task<UploadSessionDto> UploadChunkAsync(
            UploadChunkDto dto,
            CancellationToken ct = default);

        // Complete the upload and merge chunks
        Task<MediaAssetDetailDto> CompleteUploadAsync(
            Guid sessionId,
            CancellationToken ct = default);

        // Get upload session status
        Task<UploadSessionDto?> GetSessionStatusAsync(
            Guid sessionId,
            CancellationToken ct = default);

        // Cancel upload session
        Task CancelUploadAsync(
            Guid sessionId,
            CancellationToken ct = default);
    }
}

