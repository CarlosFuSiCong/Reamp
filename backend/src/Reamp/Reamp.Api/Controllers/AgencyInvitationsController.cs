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
    [Route("api/agencies")]
    [Authorize]
    public sealed class AgencyInvitationsController : ControllerBase
    {
        private readonly IInvitationAppService _invitationService;
        private readonly ILogger<AgencyInvitationsController> _logger;

        public AgencyInvitationsController(
            IInvitationAppService invitationService,
            ILogger<AgencyInvitationsController> logger)
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

        // POST /api/agencies/{agencyId}/invitations - Send agency invitation
        [HttpPost("{agencyId:guid}/invitations")]
        [RequireAgencyRole(AgencyRole.Manager)]
        public async Task<IActionResult> SendInvitation(
            Guid agencyId,
            [FromBody] SendAgencyInvitationDto dto,
            CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var invitation = await _invitationService.SendAgencyInvitationAsync(agencyId, dto, currentUserId, ct);

                _logger.LogInformation(
                    "Agency invitation sent: AgencyId={AgencyId}, Email={Email}, Role={Role}",
                    agencyId, dto.InviteeEmail, dto.TargetRole);

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

        // GET /api/agencies/{agencyId}/invitations - Get agency invitations
        [HttpGet("{agencyId:guid}/invitations")]
        public async Task<IActionResult> GetInvitations(Guid agencyId, CancellationToken ct)
        {
            try
            {
                var invitations = await _invitationService.GetAgencyInvitationsAsync(agencyId, ct);

                return Ok(ApiResponse<List<InvitationDetailDto>>.Ok(
                    invitations,
                    "Invitations retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agency invitations: {AgencyId}", agencyId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving invitations"));
            }
        }
    }
}
