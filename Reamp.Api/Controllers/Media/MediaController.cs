using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<MediaController> _logger;

        public MediaController(
            IMediaAssetAppService mediaAssetAppService,
            ILogger<MediaController> logger)
        {
            _mediaAssetAppService = mediaAssetAppService;
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

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}

