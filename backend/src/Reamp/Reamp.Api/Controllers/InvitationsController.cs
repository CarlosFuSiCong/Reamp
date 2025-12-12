using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Invitations.Dtos;
using Reamp.Application.Invitations.Services;
using Reamp.Shared;
using System.Security.Claims;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    [Authorize]
    public sealed class InvitationsController : ControllerBase
    {
        private readonly IInvitationAppService _invitationService;
        private readonly ILogger<InvitationsController> _logger;

        public InvitationsController(
            IInvitationAppService invitationService,
            ILogger<InvitationsController> logger)
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

        private string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        // GET /api/invitations/me - Get my invitations
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInvitations(CancellationToken ct)
        {
            try
            {
                var email = GetCurrentUserEmail();
                var invitations = await _invitationService.GetMyInvitationsAsync(email, ct);

                return Ok(ApiResponse<List<InvitationListDto>>.Ok(
                    invitations,
                    "Your invitations retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user invitations");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving invitations"));
            }
        }

        // GET /api/invitations/{invitationId} - Get invitation details
        [HttpGet("{invitationId:guid}")]
        public async Task<IActionResult> GetInvitationById(Guid invitationId, CancellationToken ct)
        {
            try
            {
                var invitation = await _invitationService.GetInvitationByIdAsync(invitationId, ct);

                if (invitation == null)
                    return NotFound(ApiResponse<object>.Fail("Invitation not found"));

                return Ok(ApiResponse<InvitationDetailDto>.Ok(
                    invitation,
                    "Invitation retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invitation: {InvitationId}", invitationId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving invitation"));
            }
        }

        // POST /api/invitations/{invitationId}/accept - Accept invitation
        [HttpPost("{invitationId:guid}/accept")]
        public async Task<IActionResult> AcceptInvitation(Guid invitationId, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _invitationService.AcceptInvitationAsync(invitationId, currentUserId, ct);

                _logger.LogInformation(
                    "Invitation accepted: InvitationId={InvitationId}, UserId={UserId}",
                    invitationId, currentUserId);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Invitation accepted successfully"));
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

        // POST /api/invitations/{invitationId}/reject - Reject invitation
        [HttpPost("{invitationId:guid}/reject")]
        public async Task<IActionResult> RejectInvitation(Guid invitationId, CancellationToken ct)
        {
            try
            {
                var userEmail = GetCurrentUserEmail();
                await _invitationService.RejectInvitationAsync(invitationId, userEmail, ct);

                _logger.LogInformation("Invitation rejected: InvitationId={InvitationId}", invitationId);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Invitation rejected successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // POST /api/invitations/{invitationId}/cancel - Cancel invitation (by inviter)
        [HttpPost("{invitationId:guid}/cancel")]
        public async Task<IActionResult> CancelInvitation(Guid invitationId, CancellationToken ct)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _invitationService.CancelInvitationAsync(invitationId, currentUserId, ct);

                _logger.LogInformation("Invitation cancelled: InvitationId={InvitationId}", invitationId);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Invitation cancelled successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}
