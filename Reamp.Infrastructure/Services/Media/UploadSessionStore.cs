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
        public DateTime LastActivityUtc { get; set; } // Track last activity for cleanup
        public DateTime? CompletedAtUtc { get; set; }

        // P2 Fix: Use HashSet instead of ConcurrentBag to prevent duplicate indexes
        // ConcurrentBag allows duplicates, which breaks progress calculation
        // HashSet ensures each index is added only once
        private readonly HashSet<int> _receivedChunks = new();
        private readonly object _chunksLock = new();

        // Store chunk data temporarily
        public ConcurrentDictionary<int, byte[]> ChunkData { get; set; } = new();

        // Thread-safe methods to manage received chunks
        public bool TryAddChunkIndex(int chunkIndex)
        {
            lock (_chunksLock)
            {
                return _receivedChunks.Add(chunkIndex);
            }
        }

        public bool ContainsChunkIndex(int chunkIndex)
        {
            lock (_chunksLock)
            {
                return _receivedChunks.Contains(chunkIndex);
            }
        }

        public int[] GetReceivedChunkIndexes()
        {
            lock (_chunksLock)
            {
                return _receivedChunks.ToArray();
            }
        }

        public int UploadedChunksCount
        {
            get
            {
                lock (_chunksLock)
                {
                    return _receivedChunks.Count;
                }
            }
        }

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
        Task<IEnumerable<UploadSession>> GetAllSessionsAsync(); // P1 Fix: For background cleanup
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

        public Task<IEnumerable<UploadSession>> GetAllSessionsAsync()
        {
            return Task.FromResult<IEnumerable<UploadSession>>(_sessions.Values.ToList());
        }
    }
}

