using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Accounts.Staff.Dtos;
using Reamp.Application.Accounts.Staff.Services;
using Reamp.Application.Read.Staff;
using Reamp.Domain.Accounts.Enums;
using Reamp.Shared;
using PageRequest = Reamp.Application.Read.Shared.PageRequest;

namespace Reamp.Api.Controllers.Accounts
{
    [ApiController]
    [Route("api")]
    public sealed class StaffController : ControllerBase
    {
        private readonly IStaffAppService _staffAppService;
        private readonly IStaffReadService _staffReadService;
        private readonly ILogger<StaffController> _logger;

        public StaffController(
            IStaffAppService staffAppService,
            IStaffReadService staffReadService,
            ILogger<StaffController> logger)
        {
            _staffAppService = staffAppService;
            _staffReadService = staffReadService;
            _logger = logger;
        }

        // POST /api/studios/{studioId}/staff - Add staff member
        [HttpPost("studios/{studioId:guid}/staff")]
        [Authorize]
        public async Task<IActionResult> Create(Guid studioId, [FromBody] CreateStaffDto dto, CancellationToken ct)
        {
            // Ensure DTO StudioId matches route parameter
            if (dto.StudioId != studioId)
                return BadRequest(ApiResponse<object>.Fail("StudioId in route and body must match."));

            _logger.LogInformation("Creating staff for studio: {StudioId}", studioId);

            try
            {
                var staff = await _staffAppService.CreateAsync(dto, ct);

                _logger.LogInformation("Staff created successfully: {StaffId}", staff.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = staff.Id },
                    ApiResponse<StaffDetailDto>.Ok(staff, "Staff created successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // GET /api/studios/{studioId}/staff - List staff members
        [HttpGet("studios/{studioId:guid}/staff")]
        public async Task<IActionResult> ListByStudio(
            Guid studioId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] StaffSkills? hasSkill = null,
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

            try
            {
                var result = await _staffReadService.ListByStudioAsync(studioId, search, hasSkill, pageRequest, ct);

                return Ok(ApiResponse<Reamp.Application.Read.Shared.PageResult<Reamp.Application.Read.Staff.DTOs.StaffSummaryDto>>.Ok(
                    result,
                    "Staff retrieved successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Studio not found"));
            }
        }

        // GET /api/staff/{id} - Get staff details
        [HttpGet("staff/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var staff = await _staffAppService.GetByIdAsync(id, ct);

            if (staff == null)
                return NotFound(ApiResponse<object>.Fail("Staff not found"));

            return Ok(ApiResponse<StaffDetailDto>.Ok(staff, "Staff retrieved successfully"));
        }

        // PUT /api/staff/{id}/skills - Update staff skills
        [HttpPut("staff/{id:guid}/skills")]
        [Authorize]
        public async Task<IActionResult> UpdateSkills(Guid id, [FromBody] UpdateStaffSkillsDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating skills for staff: {StaffId}", id);

            try
            {
                var staff = await _staffAppService.UpdateSkillsAsync(id, dto, ct);

                _logger.LogInformation("Staff skills updated successfully: {StaffId}", id);

                return Ok(ApiResponse<StaffDetailDto>.Ok(staff, "Staff skills updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // DELETE /api/staff/{id} - Remove staff member
        [HttpDelete("staff/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Deleting staff: {StaffId}", id);

            try
            {
                await _staffAppService.DeleteAsync(id, ct);

                _logger.LogInformation("Staff deleted successfully: {StaffId}", id);

                return Ok(ApiResponse<object?>.Ok(null, "Staff deleted successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Staff not found"));
            }
        }
    }
}

