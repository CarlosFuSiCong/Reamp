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
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            var session = await _sessionStore.GetSessionAsync(dto.UploadSessionId);
            if (session == null)
                throw new KeyNotFoundException($"Upload session {dto.UploadSessionId} not found.");

            // Security: Verify the caller owns this upload session
            if (session.UploaderUserId != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to upload chunk to session {SessionId} owned by {OwnerId}",
                    currentUserId, session.SessionId, session.UploaderUserId);
                throw new UnauthorizedAccessException($"You are not authorized to upload to this session.");
            }

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
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            var session = await _sessionStore.GetSessionAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Upload session {sessionId} not found.");

            // Security: Verify the caller owns this upload session
            if (session.UploaderUserId != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to complete session {SessionId} owned by {OwnerId}",
                    currentUserId, sessionId, session.UploaderUserId);
                throw new UnauthorizedAccessException($"You are not authorized to complete this session.");
            }

            // Idempotency: Prevent duplicate completion and re-upload
            if (session.CompletedAtUtc.HasValue)
            {
                _logger.LogWarning("Session {SessionId} already completed at {CompletedAt}, ignoring duplicate completion request",
                    sessionId, session.CompletedAtUtc.Value);
                throw new InvalidOperationException($"Upload session {sessionId} has already been completed at {session.CompletedAtUtc.Value:u}. Cannot re-process a completed session.");
            }

            if (!session.IsComplete)
                throw new InvalidOperationException($"Upload session {sessionId} is not complete. Received {session.UploadedChunksCount}/{session.TotalChunks} chunks.");

            _logger.LogInformation("Completing upload for session {SessionId}", sessionId);

            // Validate file size before allocating buffer (C# arrays require int dimensions)
            if (session.TotalSize > int.MaxValue)
            {
                _logger.LogError("File size {TotalSize} exceeds maximum supported size {MaxSize} for session {SessionId}",
                    session.TotalSize, int.MaxValue, sessionId);
                throw new InvalidOperationException($"File size ({session.TotalSize} bytes) exceeds maximum supported size ({int.MaxValue} bytes ~2GB).");
            }

            // Merge all chunks with integrity validation
            var mergedData = new byte[(int)session.TotalSize];
            long currentPosition = 0;

            for (int i = 0; i < session.TotalChunks; i++)
            {
                if (!session.ChunkData.TryGetValue(i, out var chunkData))
                    throw new InvalidOperationException($"Chunk {i} is missing.");

                // Data integrity: Verify chunk doesn't exceed remaining buffer space
                long remainingSpace = session.TotalSize - currentPosition;
                if (chunkData.Length > remainingSpace)
                {
                    _logger.LogError("Chunk {ChunkIndex} size {ChunkSize} exceeds remaining buffer space {RemainingSpace} for session {SessionId}",
                        i, chunkData.Length, remainingSpace, sessionId);
                    throw new InvalidOperationException($"Chunk {i} size ({chunkData.Length} bytes) exceeds remaining buffer space ({remainingSpace} bytes). Upload corrupted.");
                }

                Array.Copy(chunkData, 0, mergedData, currentPosition, chunkData.Length);
                currentPosition += chunkData.Length;
            }

            // Data integrity: Verify merged size matches expected total size
            if (currentPosition != session.TotalSize)
            {
                _logger.LogError("Merged data size {ActualSize} does not match expected size {ExpectedSize} for session {SessionId}",
                    currentPosition, session.TotalSize, sessionId);
                throw new InvalidOperationException($"Data integrity check failed: merged size ({currentPosition} bytes) does not match expected size ({session.TotalSize} bytes). Upload corrupted.");
            }

            _logger.LogInformation("Data integrity verified for session {SessionId}: {TotalSize} bytes", sessionId, currentPosition);

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
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            var session = await _sessionStore.GetSessionAsync(sessionId);
            if (session == null)
                return null;

            // Security: Verify the caller owns this upload session
            if (session.UploaderUserId != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to access session {SessionId} owned by {OwnerId}",
                    currentUserId, sessionId, session.UploaderUserId);
                throw new UnauthorizedAccessException($"You are not authorized to access this session.");
            }

            return MapToDto(session);
        }

        public async Task CancelUploadAsync(
            Guid sessionId,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            var session = await _sessionStore.GetSessionAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Upload session {sessionId} not found.");

            // Security: Verify the caller owns this upload session
            if (session.UploaderUserId != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to cancel session {SessionId} owned by {OwnerId}",
                    currentUserId, sessionId, session.UploaderUserId);
                throw new UnauthorizedAccessException($"You are not authorized to cancel this session.");
            }

            await _sessionStore.DeleteSessionAsync(sessionId);
            _logger.LogInformation("Upload session {SessionId} cancelled by user {UserId}", sessionId, currentUserId);
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

