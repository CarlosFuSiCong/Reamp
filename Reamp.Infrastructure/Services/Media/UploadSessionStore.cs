using System.Collections.Concurrent;

namespace Reamp.Infrastructure.Services.Media
{
    // Upload session for managing chunked uploads
    public class UploadSession
    {
        public Guid SessionId { get; set; }
        public Guid OwnerStudioId { get; set; }
        public Guid UploaderUserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public int TotalChunks { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        // Track received chunks
        public ConcurrentBag<int> ReceivedChunks { get; set; } = new();

        // Store chunk data temporarily
        public ConcurrentDictionary<int, byte[]> ChunkData { get; set; } = new();

        public int UploadedChunksCount => ReceivedChunks.Count;
        public double Progress => TotalChunks > 0 ? (UploadedChunksCount / (double)TotalChunks) * 100 : 0;
        public bool IsComplete => UploadedChunksCount >= TotalChunks;
    }

    // In-memory storage for upload sessions (for demonstration)
    // In production, use Redis or database
    public interface IUploadSessionStore
    {
        Task<UploadSession> CreateSessionAsync(UploadSession session);
        Task<UploadSession?> GetSessionAsync(Guid sessionId);
        Task UpdateSessionAsync(UploadSession session);
        Task DeleteSessionAsync(Guid sessionId);
    }

    public class InMemoryUploadSessionStore : IUploadSessionStore
    {
        private readonly ConcurrentDictionary<Guid, UploadSession> _sessions = new();

        public Task<UploadSession> CreateSessionAsync(UploadSession session)
        {
            _sessions[session.SessionId] = session;
            return Task.FromResult(session);
        }

        public Task<UploadSession?> GetSessionAsync(Guid sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return Task.FromResult(session);
        }

        public Task UpdateSessionAsync(UploadSession session)
        {
            _sessions[session.SessionId] = session;
            return Task.CompletedTask;
        }

        public Task DeleteSessionAsync(Guid sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
            return Task.CompletedTask;
        }
    }
}

