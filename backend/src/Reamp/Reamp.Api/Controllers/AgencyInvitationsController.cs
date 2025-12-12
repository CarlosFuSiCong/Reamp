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
    [Route("api/agencies")]
    [Authorize]
    public sealed class AgencyInvitationsController : ControllerBase
    {
        private readonly IInvitationAppService _invitationService;
        private readonly IMemberAppService _memberService;
        private readonly ILogger<AgencyInvitationsController> _logger;

        public AgencyInvitationsController(
            IInvitationAppService invitationService,
            IMemberAppService memberService,
            ILogger<AgencyInvitationsController> logger)
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

        // POST /api/agencies/{agencyId}/invitations - Send agency invitation
        [HttpPost("{agencyId:guid}/invitations")]
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

        // GET /api/agencies/{agencyId}/members - Get agency members
        [HttpGet("{agencyId:guid}/members")]
        public async Task<IActionResult> GetMembers(Guid agencyId, CancellationToken ct)
        {
            try
            {
                var members = await _memberService.GetAgencyMembersAsync(agencyId, ct);

                return Ok(ApiResponse<List<AgencyMemberDto>>.Ok(
                    members,
                    "Members retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agency members: {AgencyId}", agencyId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving members"));
            }
        }

        // PUT /api/agencies/{agencyId}/members/{memberId}/role - Update member role
        [HttpPut("{agencyId:guid}/members/{memberId:guid}/role")]
        public async Task<IActionResult> UpdateMemberRole(
            Guid agencyId,
            Guid memberId,
            [FromBody] UpdateAgencyMemberRoleDto dto,
            CancellationToken ct)
        {
            try
            {
                var member = await _memberService.UpdateAgencyMemberRoleAsync(agencyId, memberId, dto, ct);

                _logger.LogInformation(
                    "Agency member role updated: AgencyId={AgencyId}, MemberId={MemberId}, NewRole={NewRole}",
                    agencyId, memberId, dto.NewRole);

                return Ok(ApiResponse<AgencyMemberDto>.Ok(
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

        // DELETE /api/agencies/{agencyId}/members/{memberId} - Remove member
        [HttpDelete("{agencyId:guid}/members/{memberId:guid}")]
        public async Task<IActionResult> RemoveMember(
            Guid agencyId,
            Guid memberId,
            CancellationToken ct)
        {
            try
            {
                await _memberService.RemoveAgencyMemberAsync(agencyId, memberId, ct);

                _logger.LogInformation(
                    "Agency member removed: AgencyId={AgencyId}, MemberId={MemberId}",
                    agencyId, memberId);

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
