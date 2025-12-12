using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Invitations.Dtos;
using Reamp.Application.Invitations.Services;
using Reamp.Application.Members.Dtos;
using Reamp.Application.Members.Services;
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
        private readonly IMemberAppService _memberService;
        private readonly ILogger<StudioInvitationsController> _logger;

        public StudioInvitationsController(
            IInvitationAppService invitationService,
            IMemberAppService memberService,
            ILogger<StudioInvitationsController> logger)
        {
            _invitationService = invitationService;
            _memberService = memberService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        private string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        // POST /api/studios/{studioId}/invitations - Send studio invitation
        [HttpPost("{studioId:guid}/invitations")]
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

        // GET /api/studios/{studioId}/members - Get studio members
        [HttpGet("{studioId:guid}/members")]
        public async Task<IActionResult> GetMembers(Guid studioId, CancellationToken ct)
        {
            try
            {
                var members = await _memberService.GetStudioMembersAsync(studioId, ct);

                return Ok(ApiResponse<List<StudioMemberDto>>.Ok(
                    members,
                    "Members retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving studio members: {StudioId}", studioId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving members"));
            }
        }

        // PUT /api/studios/{studioId}/members/{memberId}/role - Update member role
        [HttpPut("{studioId:guid}/members/{memberId:guid}/role")]
        public async Task<IActionResult> UpdateMemberRole(
            Guid studioId,
            Guid memberId,
            [FromBody] UpdateStudioMemberRoleDto dto,
            CancellationToken ct)
        {
            try
            {
                var member = await _memberService.UpdateStudioMemberRoleAsync(studioId, memberId, dto, ct);

                _logger.LogInformation(
                    "Studio member role updated: StudioId={StudioId}, MemberId={MemberId}, NewRole={NewRole}",
                    studioId, memberId, dto.NewRole);

                return Ok(ApiResponse<StudioMemberDto>.Ok(
                    member,
                    "Member role updated successfully"));
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

        // DELETE /api/studios/{studioId}/members/{memberId} - Remove member
        [HttpDelete("{studioId:guid}/members/{memberId:guid}")]
        public async Task<IActionResult> RemoveMember(
            Guid studioId,
            Guid memberId,
            CancellationToken ct)
        {
            try
            {
                await _memberService.RemoveStudioMemberAsync(studioId, memberId, ct);

                _logger.LogInformation(
                    "Studio member removed: StudioId={StudioId}, MemberId={MemberId}",
                    studioId, memberId);

                return Ok(ApiResponse<object>.Ok(
                    null,
                    "Member removed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}
