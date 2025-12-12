using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Api.Attributes;
using Reamp.Application.Members.Dtos;
using Reamp.Application.Members.Services;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId:guid}/members")]
    [Authorize]
    public class AgencyMembersController : ControllerBase
    {
        private readonly IMemberAppService _memberService;

        public AgencyMembersController(IMemberAppService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Get all agency members
        /// </summary>
        [HttpGet]
        [RequireAgencyRole(AgencyRole.Member)]
        public async Task<ActionResult<List<AgencyMemberDto>>> GetMembers(
            Guid agencyId,
            CancellationToken ct)
        {
            var members = await _memberService.GetAgencyMembersAsync(agencyId, ct);
            return Ok(members);
        }

        /// <summary>
        /// Update member role
        /// </summary>
        [HttpPut("{memberId:guid}/role")]
        [RequireAgencyRole(AgencyRole.Manager)]
        public async Task<ActionResult<AgencyMemberDto>> UpdateMemberRole(
            Guid agencyId,
            Guid memberId,
            [FromBody] UpdateAgencyMemberRoleDto dto,
            CancellationToken ct)
        {
            var member = await _memberService.UpdateAgencyMemberRoleAsync(agencyId, memberId, dto, ct);
            return Ok(member);
        }

        /// <summary>
        /// Remove member from agency
        /// </summary>
        [HttpDelete("{memberId:guid}")]
        [RequireAgencyRole(AgencyRole.Manager)]
        public async Task<IActionResult> RemoveMember(
            Guid agencyId,
            Guid memberId,
            CancellationToken ct)
        {
            await _memberService.RemoveAgencyMemberAsync(agencyId, memberId, ct);
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/studios/{studioId:guid}/members")]
    [Authorize]
    public class StudioMembersController : ControllerBase
    {
        private readonly IMemberAppService _memberService;

        public StudioMembersController(IMemberAppService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Get all studio members
        /// </summary>
        [HttpGet]
        [RequireStudioRole(StudioRole.Member)]
        public async Task<ActionResult<List<StudioMemberDto>>> GetMembers(
            Guid studioId,
            CancellationToken ct)
        {
            var members = await _memberService.GetStudioMembersAsync(studioId, ct);
            return Ok(members);
        }

        /// <summary>
        /// Update member role
        /// </summary>
        [HttpPut("{memberId:guid}/role")]
        [RequireStudioRole(StudioRole.Manager)]
        public async Task<ActionResult<StudioMemberDto>> UpdateMemberRole(
            Guid studioId,
            Guid memberId,
            [FromBody] UpdateStudioMemberRoleDto dto,
            CancellationToken ct)
        {
            var member = await _memberService.UpdateStudioMemberRoleAsync(studioId, memberId, dto, ct);
            return Ok(member);
        }

        /// <summary>
        /// Remove member from studio
        /// </summary>
        [HttpDelete("{memberId:guid}")]
        [RequireStudioRole(StudioRole.Manager)]
        public async Task<IActionResult> RemoveMember(
            Guid studioId,
            Guid memberId,
            CancellationToken ct)
        {
            await _memberService.RemoveStudioMemberAsync(studioId, memberId, ct);
            return NoContent();
        }
    }
}
