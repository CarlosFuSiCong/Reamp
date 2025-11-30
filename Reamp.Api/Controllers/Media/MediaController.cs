using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Reamp.Api.Hubs;
using Reamp.Application.Media.Dtos;
using Reamp.Application.Media.Services;
using Reamp.Domain.Media.Enums;
using Reamp.Shared;
using System.Security.Claims;

namespace Reamp.Api.Controllers.Media
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediaAssetAppService _mediaAssetAppService;
        private readonly IChunkedUploadService _chunkedUploadService;
        private readonly IHubContext<UploadProgressHub> _progressHub;
        private readonly ILogger<MediaController> _logger;

        public MediaController(
            IMediaAssetAppService mediaAssetAppService,
            IChunkedUploadService chunkedUploadService,
            IHubContext<UploadProgressHub> progressHub,
            ILogger<MediaController> logger)
        {
            _mediaAssetAppService = mediaAssetAppService;
            _chunkedUploadService = chunkedUploadService;
            _progressHub = progressHub;
            _logger = logger;
        }

        // POST /api/media/upload - Upload media file
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid studioId, [FromForm] string? description, CancellationToken ct)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(ApiResponse<object>.Fail("File is required"));

                var currentUserId = GetCurrentUserId();
                _logger.LogInformation("Uploading media for studio {StudioId} by user {UserId}",
                    studioId, currentUserId);

                // Prepare DTO with file stream
                using var stream = file.OpenReadStream();
                var dto = new UploadMediaDto
                {
                    OwnerStudioId = studioId,
                    FileStream = stream,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    Description = description
                };

                var result = await _mediaAssetAppService.UploadAsync(dto, currentUserId, ct);

                return Ok(ApiResponse<MediaAssetDetailDto>.Ok(
                    result,
                    "Media uploaded successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while uploading media"));
            }
        }

        // GET /api/media/{id} - Get media details
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _mediaAssetAppService.GetByIdAsync(id, ct);

                if (result == null)
                    return NotFound(ApiResponse<object>.Fail("Media asset not found"));

                return Ok(ApiResponse<MediaAssetDetailDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting media asset {AssetId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving media"));
            }
        }

        // GET /api/media/studio/{studioId} - List media by studio
        [HttpGet("studio/{studioId:guid}")]
        public async Task<IActionResult> ListByStudio(
            Guid studioId,
            [FromQuery] MediaResourceType? resourceType = null,
            [FromQuery] MediaProcessStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            try
            {
                var result = await _mediaAssetAppService.ListByStudioAsync(
                    studioId, resourceType, status, page, pageSize, ct);

                var totalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);

                return Ok(ApiResponse<object>.Ok(
                    new
                    {
                        items = result.Items,
                        total = result.TotalCount,
                        page = result.Page,
                        pageSize = result.PageSize,
                        totalPages = totalPages
                    },
                    "Media list retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing media for studio {StudioId}", studioId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while listing media"));
            }
        }

        // POST /api/media/{id}/variants - Add variant
        [HttpPost("{id:guid}/variants")]
        public async Task<IActionResult> AddVariant(Guid id, [FromBody] AddVariantDto dto, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Adding variant '{VariantName}' to media {AssetId}",
                    dto.VariantName, id);

                var result = await _mediaAssetAppService.AddVariantAsync(id, dto, ct);

                return Ok(ApiResponse<MediaAssetDetailDto>.Ok(
                    result,
                    "Variant added successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding variant to media {AssetId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while adding variant"));
            }
        }

        // PUT /api/media/{id}/process - Trigger processing
        [HttpPut("{id:guid}/process")]
        public async Task<IActionResult> TriggerProcessing(Guid id, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Triggering processing for media {AssetId}", id);

                await _mediaAssetAppService.TriggerProcessingAsync(id, ct);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Processing triggered successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering processing for media {AssetId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while triggering processing"));
            }
        }

        // DELETE /api/media/{id} - Delete media
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Deleting media {AssetId}", id);

                await _mediaAssetAppService.DeleteAsync(id, ct);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Media deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media {AssetId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while deleting media"));
            }
        }

        // ===== Chunked Upload Endpoints =====

        // POST /api/media/chunked/initiate - Initiate chunked upload
        [HttpPost("chunked/initiate")]
        public async Task<IActionResult> InitiateChunkedUpload(
            [FromBody] InitiateChunkedUploadDto dto,
            CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var session = await _chunkedUploadService.InitiateUploadAsync(dto, currentUserId, ct);

                _logger.LogInformation("Chunked upload initiated: {SessionId}", session.SessionId);

                return Ok(ApiResponse<UploadSessionDto>.Ok(
                    session,
                    "Chunked upload session initiated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating chunked upload");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while initiating upload"));
            }
        }

        // POST /api/media/chunked/upload - Upload a chunk
        [HttpPost("chunked/upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB per chunk
        public async Task<IActionResult> UploadChunk(
            [FromForm] Guid sessionId,
            [FromForm] int chunkIndex,
            [FromForm] IFormFile chunk,
            CancellationToken ct)
        {
            try
            {
                if (chunk == null || chunk.Length == 0)
                    return BadRequest(ApiResponse<object>.Fail("Chunk is required"));

                using var stream = chunk.OpenReadStream();
                var dto = new UploadChunkDto
                {
                    UploadSessionId = sessionId,
                    ChunkIndex = chunkIndex,
                    ChunkData = stream
                };

                var session = await _chunkedUploadService.UploadChunkAsync(dto, ct);

                // Send progress update via SignalR
                var connectionId = Request.Headers["X-SignalR-ConnectionId"].ToString();
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _progressHub.Clients.Client(connectionId).SendAsync(
                        "ReceiveProgress",
                        (int)session.Progress,
                        session.FileName,
                        ct);
                }

                _logger.LogInformation("Chunk {ChunkIndex} uploaded for session {SessionId}",
                    chunkIndex, sessionId);

                return Ok(ApiResponse<UploadSessionDto>.Ok(
                    session,
                    $"Chunk {chunkIndex + 1}/{session.TotalChunks} uploaded"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading chunk");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while uploading chunk"));
            }
        }

        // POST /api/media/chunked/complete - Complete chunked upload
        [HttpPost("chunked/complete/{sessionId:guid}")]
        public async Task<IActionResult> CompleteChunkedUpload(Guid sessionId, CancellationToken ct)
        {
            try
            {
                var mediaAsset = await _chunkedUploadService.CompleteUploadAsync(sessionId, ct);

                // Send completion notification via SignalR
                var connectionId = Request.Headers["X-SignalR-ConnectionId"].ToString();
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _progressHub.Clients.Client(connectionId).SendAsync(
                        "UploadComplete",
                        mediaAsset.OriginalFileName,
                        mediaAsset.Id,
                        ct);
                }

                _logger.LogInformation("Chunked upload completed: {AssetId}", mediaAsset.Id);

                return Ok(ApiResponse<MediaAssetDetailDto>.Ok(
                    mediaAsset,
                    "Upload completed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing chunked upload");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while completing upload"));
            }
        }

        // GET /api/media/chunked/status/{sessionId} - Get upload session status
        [HttpGet("chunked/status/{sessionId:guid}")]
        public async Task<IActionResult> GetChunkedUploadStatus(Guid sessionId, CancellationToken ct)
        {
            try
            {
                var session = await _chunkedUploadService.GetSessionStatusAsync(sessionId, ct);

                if (session == null)
                    return NotFound(ApiResponse<object>.Fail("Upload session not found"));

                return Ok(ApiResponse<UploadSessionDto>.Ok(session));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upload session status");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // DELETE /api/media/chunked/cancel/{sessionId} - Cancel chunked upload
        [HttpDelete("chunked/cancel/{sessionId:guid}")]
        public async Task<IActionResult> CancelChunkedUpload(Guid sessionId, CancellationToken ct)
        {
            try
            {
                await _chunkedUploadService.CancelUploadAsync(sessionId, ct);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Upload cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling upload");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}

