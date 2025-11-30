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

        // Upload a chunk (requires user identity verification)
        Task<UploadSessionDto> UploadChunkAsync(
            UploadChunkDto dto,
            Guid currentUserId,
            CancellationToken ct = default);

        // Complete the upload and merge chunks (requires user identity verification)
        Task<MediaAssetDetailDto> CompleteUploadAsync(
            Guid sessionId,
            Guid currentUserId,
            CancellationToken ct = default);

        // Get upload session status (requires user identity verification)
        Task<UploadSessionDto?> GetSessionStatusAsync(
            Guid sessionId,
            Guid currentUserId,
            CancellationToken ct = default);

        // Cancel upload session (requires user identity verification)
        Task CancelUploadAsync(
            Guid sessionId,
            Guid currentUserId,
            CancellationToken ct = default);
    }
}

