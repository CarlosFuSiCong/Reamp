using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Api.Attributes;
using Reamp.Application.Members.Dtos;
using Reamp.Application.Members.Services;
using Reamp.Domain.Accounts.Enums;
using Reamp.Shared;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId:guid}/members")]
    [Authorize]
    public class AgencyMembersController : ControllerBase
    {
        private readonly IMemberAppService _memberService;
        private readonly ILogger<AgencyMembersController> _logger;

        public AgencyMembersController(
            IMemberAppService memberService,
            ILogger<AgencyMembersController> logger)
        {
            _memberService = memberService;
            _logger = logger;
        }

        /// <summary>
        /// Get all agency members
        /// </summary>
        [HttpGet]
        [RequireAgencyRole(AgencyRole.Member)]
        public async Task<IActionResult> GetMembers(
            Guid agencyId,
            CancellationToken ct)
        {
            try
            {
                var members = await _memberService.GetAgencyMembersAsync(agencyId, ct);
                return Ok(ApiResponse<List<AgencyMemberDto>>.Ok(
                    members,
                    "Agency members retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agency members: {AgencyId}", agencyId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving agency members"));
            }
        }

        /// <summary>
        /// Update member role
        /// </summary>
        [HttpPut("{memberId:guid}/role")]
        [RequireAgencyRole(AgencyRole.Manager)]
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agency member role: {AgencyId}, {MemberId}", agencyId, memberId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while updating member role"));
            }
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
            try
            {
                await _memberService.RemoveAgencyMemberAsync(agencyId, memberId, ct);
                
                _logger.LogInformation(
                    "Agency member removed: AgencyId={AgencyId}, MemberId={MemberId}",
                    agencyId, memberId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing agency member: {AgencyId}, {MemberId}", agencyId, memberId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while removing member"));
            }
        }
    }

    [ApiController]
    [Route("api/studios/{studioId:guid}/members")]
    [Authorize]
    public class StudioMembersController : ControllerBase
    {
        private readonly IMemberAppService _memberService;
        private readonly ILogger<StudioMembersController> _logger;

        public StudioMembersController(
            IMemberAppService memberService,
            ILogger<StudioMembersController> logger)
        {
            _memberService = memberService;
            _logger = logger;
        }

        /// <summary>
        /// Get all studio members
        /// </summary>
        [HttpGet]
        [RequireStudioRole(StudioRole.Member)]
        public async Task<IActionResult> GetMembers(
            Guid studioId,
            CancellationToken ct)
        {
            try
            {
                var members = await _memberService.GetStudioMembersAsync(studioId, ct);
                return Ok(ApiResponse<List<StudioMemberDto>>.Ok(
                    members,
                    "Studio members retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving studio members: {StudioId}", studioId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving studio members"));
            }
        }

        /// <summary>
        /// Update member role
        /// </summary>
        [HttpPut("{memberId:guid}/role")]
        [RequireStudioRole(StudioRole.Manager)]
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating studio member role: {StudioId}, {MemberId}", studioId, memberId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while updating member role"));
            }
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
            try
            {
                await _memberService.RemoveStudioMemberAsync(studioId, memberId, ct);
                
                _logger.LogInformation(
                    "Studio member removed: StudioId={StudioId}, MemberId={MemberId}",
                    studioId, memberId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing studio member: {StudioId}, {MemberId}", studioId, memberId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while removing member"));
            }
        }
    }
}
