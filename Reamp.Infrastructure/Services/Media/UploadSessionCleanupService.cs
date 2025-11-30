using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reamp.Infrastructure.Configuration;

namespace Reamp.Infrastructure.Services.Media
{
    /// <summary>
    /// Background service that periodically cleans up abandoned chunked upload sessions.
    /// P1 Fix: Prevents memory leaks from incomplete uploads that never call complete/cancel.
    /// </summary>
    public sealed class UploadSessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UploadSessionCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval;
        private readonly TimeSpan _sessionTimeout;

        public UploadSessionCleanupService(
            IServiceProvider serviceProvider,
            ILogger<UploadSessionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            
            // Run cleanup every 5 minutes
            _cleanupInterval = TimeSpan.FromMinutes(5);
            
            // Sessions inactive for 30 minutes are considered abandoned
            _sessionTimeout = TimeSpan.FromMinutes(30);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Upload session cleanup service started. " +
                "Cleanup interval: {CleanupInterval}, Session timeout: {SessionTimeout}",
                _cleanupInterval, _sessionTimeout);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    await CleanupAbandonedSessionsAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when stopping
                    _logger.LogInformation("Upload session cleanup service is stopping.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during upload session cleanup");
                }
            }
        }

        private async Task CleanupAbandonedSessionsAsync(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var sessionStore = scope.ServiceProvider.GetRequiredService<IUploadSessionStore>();

            var allSessions = await sessionStore.GetAllSessionsAsync();
            var now = DateTime.UtcNow;
            var cleanedCount = 0;
            var totalMemoryFreed = 0L;

            foreach (var session in allSessions)
            {
                // Skip completed sessions (they have their own cleanup)
                if (session.CompletedAtUtc.HasValue)
                    continue;

                // Check if session is abandoned (no activity for sessionTimeout)
                var inactiveFor = now - session.LastActivityUtc;
                if (inactiveFor > _sessionTimeout)
                {
                    // Calculate memory being freed
                    var sessionMemory = session.ChunkData.Values.Sum(chunk => (long)chunk.Length);
                    totalMemoryFreed += sessionMemory;

                    await sessionStore.DeleteSessionAsync(session.SessionId);
                    cleanedCount++;

                    _logger.LogInformation(
                        "Cleaned up abandoned upload session {SessionId}. " +
                        "Inactive for: {InactiveFor}, Uploaded chunks: {UploadedChunks}/{TotalChunks}, " +
                        "Memory freed: {MemoryFreed} bytes",
                        session.SessionId, inactiveFor, session.UploadedChunksCount, 
                        session.TotalChunks, sessionMemory);
                }
            }

            if (cleanedCount > 0)
            {
                _logger.LogInformation(
                    "Upload session cleanup completed. " +
                    "Cleaned sessions: {CleanedCount}, Total memory freed: {TotalMemoryFreed} bytes ({MemoryMB} MB)",
                    cleanedCount, totalMemoryFreed, totalMemoryFreed / (1024.0 * 1024.0));
            }
        }
    }
}

