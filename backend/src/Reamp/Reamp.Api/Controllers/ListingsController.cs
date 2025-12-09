using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reamp.Application.Authentication;
using Reamp.Application.Listings.Dtos;
using Reamp.Application.Listings.Services;
using Reamp.Application.Read.Listings;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Listings.Enums;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/listings")]
    public sealed class ListingsController : ControllerBase
    {
        private readonly IListingAppService _appService;
        private readonly IListingReadService _readService;
        private readonly ILogger<ListingsController> _logger;

        public ListingsController(IListingAppService appService, IListingReadService readService, ILogger<ListingsController> logger)
        {
            _appService = appService;
            _readService = readService;
            _logger = logger;
        }

        // Create listing - requires Client or Admin role
        [HttpPost]
        [Authorize(Policy = AuthPolicies.RequireClientOrAdmin)]
        public async Task<IActionResult> Create([FromBody] CreateListingDto dto, CancellationToken ct)
        {
            var id = await _appService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetDetail), new { id }, new { id });
        }

        // List listings - public access
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List(
            [FromQuery] Guid? agencyId,
            [FromQuery] ListingStatus? status,
            [FromQuery] ListingType? type,
            [FromQuery] PropertyType? property,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool desc = false,
            CancellationToken ct = default)
        {
            var pageRequest = new PageRequest 
            { 
                Page = page, 
                PageSize = pageSize, 
                SortBy = sortBy, 
                Desc = desc 
            };

            var result = await _readService.ListAsync(agencyId, status, type, property, keyword, pageRequest, ct);
            return Ok(result);
        }

        // Get listing detail - public access
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetail([FromRoute] Guid id, CancellationToken ct)
        {
            var listing = await _readService.GetDetailAsync(id, ct);
            return listing is null ? NotFound() : Ok(listing);
        }

        // Get editor detail - requires Client or Admin
        [HttpGet("{id:guid}/editor")]
        [Authorize(Policy = AuthPolicies.RequireClientOrAdmin)]
        public async Task<IActionResult> GetEditorDetail([FromRoute] Guid id, CancellationToken ct)
        {
            var listing = await _readService.GetEditorDetailAsync(id, ct);
            return listing is null ? NotFound() : Ok(listing);
        }

        // Update listing details - requires Client or Admin
        [HttpPut("{id:guid}")]
        [Authorize(Policy = AuthPolicies.RequireClientOrAdmin)]
        public async Task<IActionResult> UpdateDetails([FromRoute] Guid id, [FromBody] UpdateListingDetailsDto dto, CancellationToken ct)
        {
            await _appService.UpdateDetailsAsync(id, dto, ct);
            return NoContent();
        }

        // Publish listing - requires Client or Admin
        [HttpPost("{id:guid}/publish")]
        [Authorize(Policy = AuthPolicies.RequireClientOrAdmin)]
        public async Task<IActionResult> Publish([FromRoute] Guid id, CancellationToken ct)
        {
            _logger.LogInformation("API request to publish listing {ListingId}", id);
            try
            {
                await _appService.PublishAsync(id, ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish listing {ListingId}", id);
                throw;
            }
        }

        // Archive listing - requires Client or Admin
        [HttpPost("{id:guid}/archive")]
        [Authorize(Policy = AuthPolicies.RequireClientOrAdmin)]
        public async Task<IActionResult> Archive([FromRoute] Guid id, CancellationToken ct)
        {
            _logger.LogInformation("API request to archive listing {ListingId}", id);
            try
            {
                await _appService.ArchiveAsync(id, ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to archive listing {ListingId}", id);
                throw;
            }
        }

        [HttpPost("{id:guid}/agents")]
        public async Task<IActionResult> CreateAgent([FromRoute] Guid id, [FromBody] CreateListingAgentDto dto, CancellationToken ct)
        {
            var agentId = await _appService.CreateAgentAsync(id, dto, ct);
            return CreatedAtAction(nameof(GetDetail), new { id }, new { agentId });
        }

        [HttpPost("{id:guid}/agents/{agentUserId:guid}/assign/{agentSnapshotId:guid}")]
        public async Task<IActionResult> AssignAgent([FromRoute] Guid id, [FromRoute] Guid agentUserId, [FromRoute] Guid agentSnapshotId, CancellationToken ct)
        {
            await _appService.AssignAgentAsync(id, agentUserId, agentSnapshotId, ct);
            return NoContent();
        }

        [HttpDelete("{id:guid}/agents/primary")]
        public async Task<IActionResult> UnassignPrimaryAgent([FromRoute] Guid id, CancellationToken ct)
        {
            await _appService.UnassignPrimaryAgentAsync(id, ct);
            return NoContent();
        }

        [HttpDelete("{id:guid}/agents/{agentSnapshotId:guid}")]
        public async Task<IActionResult> RemoveAgent([FromRoute] Guid id, [FromRoute] Guid agentSnapshotId, CancellationToken ct)
        {
            await _appService.RemoveAgentAsync(id, agentSnapshotId, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/agents/reorder")]
        public async Task<IActionResult> ReorderAgents([FromRoute] Guid id, [FromBody] ReorderAgentsDto dto, CancellationToken ct)
        {
            await _appService.ReorderAgentsAsync(id, dto, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/media/reorder")]
        public async Task<IActionResult> ReorderMedia([FromRoute] Guid id, [FromBody] ReorderMediaDto dto, CancellationToken ct)
        {
            await _appService.ReorderMediaAsync(id, dto, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/media/visibility")]
        public async Task<IActionResult> SetMediaVisibility([FromRoute] Guid id, [FromBody] SetMediaVisibilityDto dto, CancellationToken ct)
        {
            await _appService.SetMediaVisibilityAsync(id, dto, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/media/{mediaId:guid}/cover")]
        public async Task<IActionResult> SetCover([FromRoute] Guid id, [FromRoute] Guid mediaId, CancellationToken ct)
        {
            await _appService.SetCoverAsync(id, mediaId, ct);
            return NoContent();
        }

    }
}
