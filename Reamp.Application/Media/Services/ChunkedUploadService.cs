using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reamp.Application.Media.Dtos;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Infrastructure.Configuration;
using Reamp.Infrastructure.Services.Media;

namespace Reamp.Application.Media.Services
{
    public sealed class ChunkedUploadService : IChunkedUploadService
    {
        private readonly IUploadSessionStore _sessionStore;
        private readonly IMediaAssetAppService _mediaAssetAppService;
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<ChunkedUploadService> _logger;
        private readonly MediaUploadSettings _uploadSettings;

        public ChunkedUploadService(
            IUploadSessionStore sessionStore,
            IMediaAssetAppService mediaAssetAppService,
            IStaffRepository staffRepository,
            IOptions<MediaUploadSettings> uploadSettings,
            ILogger<ChunkedUploadService> logger)
        {
            _sessionStore = sessionStore;
            _mediaAssetAppService = mediaAssetAppService;
            _staffRepository = staffRepository;
            _uploadSettings = uploadSettings.Value;
            _logger = logger;
        }

        public async Task<UploadSessionDto> InitiateUploadAsync(
            InitiateChunkedUploadDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            // Security: Verify user is a staff member of the target studio
            // This prevents cross-tenant session creation and memory-based DoS attacks
            var isStaffMember = await _staffRepository.IsApplicationUserStaffOfStudioAsync(currentUserId, dto.OwnerStudioId, ct);
            if (!isStaffMember)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to initiate chunked upload for Studio {StudioId} but is not a staff member",
                    currentUserId, dto.OwnerStudioId);
                throw new UnauthorizedAccessException("You are not authorized to upload media to this studio. You must be a staff member.");
            }

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
                CreatedAtUtc = DateTime.UtcNow,
                LastActivityUtc = DateTime.UtcNow // P1 Fix: Track last activity for cleanup
            };

            await _sessionStore.CreateSessionAsync(session);

            _logger.LogInformation("Chunked upload session initiated: {SessionId} for Studio {StudioId} by User {UserId}", 
                session.SessionId, session.OwnerStudioId, currentUserId);

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

            // Security: Validate TotalSize against configured limits BEFORE buffering chunk data
            // This prevents memory exhaustion DoS where attacker uploads gigabytes before completion is rejected
            var contentType = session.ContentType.ToLowerInvariant();
            var isImage = contentType.StartsWith("image/");
            var isVideo = contentType.StartsWith("video/");
            var maxConfiguredSize = isImage ? _uploadSettings.MaxImageSizeBytes : _uploadSettings.MaxVideoSizeBytes;
            
            if (session.TotalSize > maxConfiguredSize)
            {
                _logger.LogError(
                    "Session {SessionId} TotalSize {TotalSize} exceeds configured max {MaxSize} for {FileType}. Rejecting chunk upload.",
                    session.SessionId, session.TotalSize, maxConfiguredSize, isImage ? "image" : "video");
                throw new InvalidOperationException(
                    $"File size ({session.TotalSize} bytes) exceeds maximum allowed ({maxConfiguredSize} bytes). Upload rejected.");
            }

            // P2 Fix: Use atomic check-and-add for duplicate detection
            // Previous ConcurrentBag.Contains + Add was not atomic, allowing parallel retries
            // to add the same index multiple times, breaking progress calculation
            if (session.ContainsChunkIndex(dto.ChunkIndex))
            {
                _logger.LogWarning("Chunk {ChunkIndex} already uploaded for session {SessionId}",
                    dto.ChunkIndex, session.SessionId);
                return MapToDto(session);
            }

            // P2 Fix: Validate chunk index is within expected range [0, TotalChunks-1]
            // Out-of-range indexes cause false progress and spurious "missing chunk" errors at completion
            if (dto.ChunkIndex < 0 || dto.ChunkIndex >= session.TotalChunks)
            {
                _logger.LogError(
                    "Chunk index {ChunkIndex} is out of range for session {SessionId}. Valid range: [0, {MaxIndex}]",
                    dto.ChunkIndex, session.SessionId, session.TotalChunks - 1);
                throw new InvalidOperationException(
                    $"Chunk index {dto.ChunkIndex} is out of range. Expected index between 0 and {session.TotalChunks - 1}.");
            }

            // Read chunk data into memory
            using var memoryStream = new MemoryStream();
            await dto.ChunkData.CopyToAsync(memoryStream, ct);
            var chunkBytes = memoryStream.ToArray();

            // Security: Validate chunk size and cumulative size against declared TotalSize
            // This prevents malicious clients from declaring small TotalSize but uploading gigabytes
            var currentBufferedSize = session.ChunkData.Values.Sum(c => (long)c.Length);
            var newBufferedSize = currentBufferedSize + chunkBytes.Length;
            
            if (newBufferedSize > session.TotalSize)
            {
                _logger.LogError(
                    "Chunk {ChunkIndex} for session {SessionId} would exceed declared TotalSize. " +
                    "Current buffered: {CurrentSize} bytes, chunk size: {ChunkSize} bytes, declared total: {TotalSize} bytes",
                    dto.ChunkIndex, session.SessionId, currentBufferedSize, chunkBytes.Length, session.TotalSize);
                throw new InvalidOperationException(
                    $"Cumulative chunk data ({newBufferedSize} bytes) exceeds declared file size ({session.TotalSize} bytes). Upload rejected.");
            }

            // P2 Fix: Atomically add chunk index to prevent duplicates from parallel retries
            // HashSet.Add returns false if already present, preventing duplicate indexes
            // Store chunk
            session.ChunkData[dto.ChunkIndex] = chunkBytes;
            session.TryAddChunkIndex(dto.ChunkIndex);
            
            // P1 Fix: Update last activity timestamp for cleanup service
            session.LastActivityUtc = DateTime.UtcNow;

            await _sessionStore.UpdateSessionAsync(session);

            _logger.LogInformation("Chunk {ChunkIndex}/{TotalChunks} uploaded for session {SessionId}. Buffered: {BufferedSize}/{TotalSize} bytes",
                dto.ChunkIndex + 1, session.TotalChunks, session.SessionId, newBufferedSize, session.TotalSize);

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

            // Security: Validate file size against configured upload limits BEFORE allocating buffer
            // This prevents OOM attacks where attacker initiates large TotalSize with minimal chunks
            var contentType = session.ContentType.ToLowerInvariant();
            var isImage = contentType.StartsWith("image/");
            var isVideo = contentType.StartsWith("video/");
            var maxConfiguredSize = isImage ? _uploadSettings.MaxImageSizeBytes : _uploadSettings.MaxVideoSizeBytes;
            
            if (session.TotalSize > maxConfiguredSize)
            {
                _logger.LogError(
                    "Session {SessionId} TotalSize {TotalSize} exceeds configured max {MaxSize} for {FileType}",
                    sessionId, session.TotalSize, maxConfiguredSize, isImage ? "image" : "video");
                throw new InvalidOperationException(
                    $"File size ({session.TotalSize} bytes) exceeds maximum allowed ({maxConfiguredSize} bytes).");
            }

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
            session.LastActivityUtc = DateTime.UtcNow; // P1 Fix: Update activity timestamp
            await _sessionStore.UpdateSessionAsync(session);

            // P1 Fix: Background cleanup service will handle session deletion
            // No need for fire-and-forget task that can't be tracked or logged properly

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

