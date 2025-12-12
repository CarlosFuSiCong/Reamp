using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Api.Attributes;
using Reamp.Application.Invitations.Dtos;
using Reamp.Application.Invitations.Services;
using Reamp.Domain.Accounts.Enums;
using Reamp.Shared;
using System.Security.Claims;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/studios")]
    [Authorize]
    public sealed class StudioInvitationsController : ControllerBase
    {
        private readonly IInvitationAppService _invitationService;
        private readonly ILogger<StudioInvitationsController> _logger;

        public StudioInvitationsController(
            IInvitationAppService invitationService,
            ILogger<StudioInvitationsController> logger)
        {
            _invitationService = invitationService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID claim not found.");
            
            return Guid.Parse(userIdClaim);
        }

        // POST /api/studios/{studioId}/invitations - Send studio invitation
        [HttpPost("{studioId:guid}/invitations")]
        [RequireStudioRole(StudioRole.Manager)]
        public async Task<IActionResult> SendInvitation(
            Guid studioId,
            [FromBody] SendStudioInvitationDto dto,
            CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var invitation = await _invitationService.SendStudioInvitationAsync(studioId, dto, currentUserId, ct);

                _logger.LogInformation(
                    "Studio invitation sent: StudioId={StudioId}, Email={Email}, Role={Role}",
                    studioId, dto.InviteeEmail, dto.TargetRole);

                return Ok(ApiResponse<InvitationDetailDto>.Ok(
                    invitation,
                    "Invitation sent successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // GET /api/studios/{studioId}/invitations - Get studio invitations
        [HttpGet("{studioId:guid}/invitations")]
        [RequireStudioRole(StudioRole.Member)]
        public async Task<IActionResult> GetInvitations(Guid studioId, CancellationToken ct)
        {
            try
            {
                var invitations = await _invitationService.GetStudioInvitationsAsync(studioId, ct);

                return Ok(ApiResponse<List<InvitationDetailDto>>.Ok(
                    invitations,
                    "Invitations retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving studio invitations: {StudioId}", studioId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving invitations"));
            }
        }
    }
}
