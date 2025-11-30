using Microsoft.Extensions.Logging;
using Reamp.Application.Media.Dtos;
using Reamp.Infrastructure.Services.Media;

namespace Reamp.Application.Media.Services
{
    public sealed class ChunkedUploadService : IChunkedUploadService
    {
        private readonly IUploadSessionStore _sessionStore;
        private readonly IMediaAssetAppService _mediaAssetAppService;
        private readonly ILogger<ChunkedUploadService> _logger;

        public ChunkedUploadService(
            IUploadSessionStore sessionStore,
            IMediaAssetAppService mediaAssetAppService,
            ILogger<ChunkedUploadService> logger)
        {
            _sessionStore = sessionStore;
            _mediaAssetAppService = mediaAssetAppService;
            _logger = logger;
        }

        public async Task<UploadSessionDto> InitiateUploadAsync(
            InitiateChunkedUploadDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            var session = new UploadSession
            {
                SessionId = Guid.NewGuid(),
                OwnerStudioId = dto.OwnerStudioId,
                UploaderUserId = currentUserId,
                FileName = dto.FileName,
                ContentType = dto.ContentType,
                TotalSize = dto.TotalSize,
                TotalChunks = dto.TotalChunks,
                Description = dto.Description,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _sessionStore.CreateSessionAsync(session);

            _logger.LogInformation("Chunked upload session initiated: {SessionId}", session.SessionId);

            return MapToDto(session);
        }

        public async Task<UploadSessionDto> UploadChunkAsync(
            UploadChunkDto dto,
            CancellationToken ct = default)
        {
            var session = await _sessionStore.GetSessionAsync(dto.UploadSessionId);
            if (session == null)
                throw new KeyNotFoundException($"Upload session {dto.UploadSessionId} not found.");

            if (session.ReceivedChunks.Contains(dto.ChunkIndex))
            {
                _logger.LogWarning("Chunk {ChunkIndex} already uploaded for session {SessionId}",
                    dto.ChunkIndex, session.SessionId);
                return MapToDto(session);
            }

            // Read chunk data
            using var memoryStream = new MemoryStream();
            await dto.ChunkData.CopyToAsync(memoryStream, ct);
            var chunkBytes = memoryStream.ToArray();

            // Store chunk
            session.ChunkData[dto.ChunkIndex] = chunkBytes;
            session.ReceivedChunks.Add(dto.ChunkIndex);

            await _sessionStore.UpdateSessionAsync(session);

            _logger.LogInformation("Chunk {ChunkIndex}/{TotalChunks} uploaded for session {SessionId}",
                dto.ChunkIndex + 1, session.TotalChunks, session.SessionId);

            return MapToDto(session);
        }

        public async Task<MediaAssetDetailDto> CompleteUploadAsync(
            Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await _sessionStore.GetSessionAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Upload session {sessionId} not found.");

            if (!session.IsComplete)
                throw new InvalidOperationException($"Upload session {sessionId} is not complete. Received {session.UploadedChunksCount}/{session.TotalChunks} chunks.");

            _logger.LogInformation("Completing upload for session {SessionId}", sessionId);

            // Merge all chunks
            var mergedData = new byte[session.TotalSize];
            long currentPosition = 0;

            for (int i = 0; i < session.TotalChunks; i++)
            {
                if (!session.ChunkData.TryGetValue(i, out var chunkData))
                    throw new InvalidOperationException($"Chunk {i} is missing.");

                Array.Copy(chunkData, 0, mergedData, currentPosition, chunkData.Length);
                currentPosition += chunkData.Length;
            }

            // Create a stream from merged data
            using var mergedStream = new MemoryStream(mergedData);

            // Upload to Cloudinary via MediaAssetAppService
            var uploadDto = new UploadMediaDto
            {
                OwnerStudioId = session.OwnerStudioId,
                FileStream = mergedStream,
                FileName = session.FileName,
                ContentType = session.ContentType,
                FileSize = session.TotalSize,
                Description = session.Description
            };

            var mediaAsset = await _mediaAssetAppService.UploadAsync(
                uploadDto,
                session.UploaderUserId,
                ct);

            // Mark session as complete
            session.CompletedAtUtc = DateTime.UtcNow;
            await _sessionStore.UpdateSessionAsync(session);

            // Clean up session after a delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                await _sessionStore.DeleteSessionAsync(sessionId);
            });

            _logger.LogInformation("Upload completed for session {SessionId}, MediaAsset {AssetId}",
                sessionId, mediaAsset.Id);

            return mediaAsset;
        }

        public async Task<UploadSessionDto?> GetSessionStatusAsync(
            Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await _sessionStore.GetSessionAsync(sessionId);
            return session == null ? null : MapToDto(session);
        }

        public async Task CancelUploadAsync(
            Guid sessionId,
            CancellationToken ct = default)
        {
            await _sessionStore.DeleteSessionAsync(sessionId);
            _logger.LogInformation("Upload session {SessionId} cancelled", sessionId);
        }

        private UploadSessionDto MapToDto(UploadSession session)
        {
            return new UploadSessionDto
            {
                SessionId = session.SessionId,
                FileName = session.FileName,
                TotalSize = session.TotalSize,
                TotalChunks = session.TotalChunks,
                UploadedChunks = session.UploadedChunksCount,
                Progress = session.Progress,
                CreatedAtUtc = session.CreatedAtUtc,
                CompletedAtUtc = session.CompletedAtUtc
            };
        }
    }
}

