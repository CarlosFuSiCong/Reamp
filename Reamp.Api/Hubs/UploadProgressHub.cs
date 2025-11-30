using Microsoft.AspNetCore.SignalR;

namespace Reamp.Api.Hubs
{
    // SignalR Hub for real-time upload progress updates
    public class UploadProgressHub : Hub
    {
        // Send progress update to specific user
        public async Task SendProgress(string connectionId, int progress, string fileName)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveProgress", progress, fileName);
        }

        // Notify upload complete
        public async Task SendComplete(string connectionId, string fileName, Guid assetId)
        {
            await Clients.Client(connectionId).SendAsync("UploadComplete", fileName, assetId);
        }

        // Notify upload error
        public async Task SendError(string connectionId, string fileName, string error)
        {
            await Clients.Client(connectionId).SendAsync("UploadError", fileName, error);
        }

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

