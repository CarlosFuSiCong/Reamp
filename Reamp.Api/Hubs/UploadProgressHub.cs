using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Reamp.Api.Hubs
{
    // SignalR Hub for real-time upload progress updates
    // Security: Require authentication to prevent unauthorized access
    [Authorize]
    public class UploadProgressHub : Hub
    {
        // Note: These methods are server-side only. Clients should not call them directly.
        // Progress updates should be sent from server-side code (e.g., ChunkedUploadService)
        // using IHubContext<UploadProgressHub> instead of allowing client-to-client messaging.

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}

