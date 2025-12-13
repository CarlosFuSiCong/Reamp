using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Application.Accounts.Agencies.Services;
using Reamp.Application.Read.Agencies;
using Reamp.Application.Read.Shared;
using Reamp.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Reamp.Api.Controllers.Accounts
{
    [ApiController]
    [Route("api/agencies")]
    public sealed class AgenciesController : ControllerBase
    {
        private readonly IAgencyAppService _agencyAppService;
        private readonly IAgencyReadService _agencyReadService;
        private readonly ILogger<AgenciesController> _logger;

        public AgenciesController(
            IAgencyAppService agencyAppService,
            IAgencyReadService agencyReadService,
            ILogger<AgenciesController> logger)
        {
            _agencyAppService = agencyAppService;
            _agencyReadService = agencyReadService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        // GET /api/agencies - List agencies with pagination and search
        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool desc = false,
            CancellationToken ct = default)
        {
            var pageRequest = new Application.Read.Shared.PageRequest
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                Desc = desc
            };

            var result = await _agencyReadService.ListAsync(search, pageRequest, ct);

            return Ok(ApiResponse<Application.Read.Shared.PageResult<Application.Read.Agencies.DTOs.AgencySummaryDto>>.Ok(
                result,
                "Agencies retrieved successfully"));
        }

        // GET /api/agencies/{id} - Get agency by ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var agency = await _agencyReadService.GetDetailAsync(id, ct);

            if (agency == null)
                return NotFound(ApiResponse<object>.Fail("Agency not found"));

            return Ok(ApiResponse<Application.Read.Agencies.DTOs.AgencyDetailDto>.Ok(
                agency,
                "Agency retrieved successfully"));
        }

        // GET /api/agencies/slug/{slug} - Get agency by slug
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
        {
            var agency = await _agencyReadService.GetDetailBySlugAsync(slug, ct);

            if (agency == null)
                return NotFound(ApiResponse<object>.Fail("Agency not found"));

            return Ok(ApiResponse<Application.Read.Agencies.DTOs.AgencyDetailDto>.Ok(
                agency,
                "Agency retrieved successfully"));
        }

        // POST /api/agencies - Create new agency
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAgencyDto dto, CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();

            _logger.LogInformation("Creating agency: {Name} by user {UserId}", dto.Name, currentUserId);

            try
            {
                var agency = await _agencyAppService.CreateAsync(dto, currentUserId, ct);

                _logger.LogInformation("Agency created successfully: {AgencyId}", agency.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = agency.Id },
                    ApiResponse<Application.Accounts.Agencies.Dtos.AgencyDetailDto>.Ok(
                        agency,
                        "Agency created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // PUT /api/agencies/{id} - Update agency
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAgencyDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating agency: {AgencyId}", id);

            try
            {
                var agency = await _agencyAppService.UpdateAsync(id, dto, ct);

                _logger.LogInformation("Agency updated successfully: {AgencyId}", id);

                return Ok(ApiResponse<Application.Accounts.Agencies.Dtos.AgencyDetailDto>.Ok(
                    agency,
                    "Agency updated successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Agency not found"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // PUT /api/agencies/{id}/logo - Update agency logo
        [HttpPut("{id:guid}/logo")]
        [Authorize]
        public async Task<IActionResult> UpdateLogo(Guid id, [FromBody] UpdateAgencyLogoDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating agency logo: {AgencyId}", id);

            try
            {
                var agency = await _agencyAppService.UpdateLogoAsync(id, dto.LogoAssetId, ct);

                _logger.LogInformation("Agency logo updated successfully: {AgencyId}", id);

                return Ok(ApiResponse<Application.Accounts.Agencies.Dtos.AgencyDetailDto>.Ok(
                    agency,
                    "Agency logo updated successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Agency not found"));
            }
        }

        // DELETE /api/agencies/{id} - Delete agency (soft delete)
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Deleting agency: {AgencyId}", id);

            try
            {
                await _agencyAppService.DeleteAsync(id, ct);

                _logger.LogInformation("Agency deleted successfully: {AgencyId}", id);

                return Ok(ApiResponse<object?>.Ok(null, "Agency deleted successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Agency not found"));
            }
        }

        // Branch Management Endpoints

        // POST /api/agencies/{id}/branches - Create new branch
        [HttpPost("{id:guid}/branches")]
        [Authorize]
        public async Task<IActionResult> CreateBranch(
            Guid id,
            [FromBody] CreateAgencyBranchDto dto,
            CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();

            _logger.LogInformation("Creating branch for agency: {AgencyId}", id);

            try
            {
                var branch = await _agencyAppService.CreateBranchAsync(id, dto, currentUserId, ct);

                _logger.LogInformation("Branch created successfully: {BranchId}", branch.Id);

                return CreatedAtAction(
                    nameof(GetBranch),
                    new { id, branchId = branch.Id },
                    ApiResponse<AgencyBranchDetailDto>.Ok(
                        branch,
                        "Branch created successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Agency not found"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // GET /api/agencies/{id}/branches - List branches
        [HttpGet("{id:guid}/branches")]
        public async Task<IActionResult> ListBranches(Guid id, CancellationToken ct)
        {
            try
            {
                var branches = await _agencyAppService.ListBranchesAsync(id, ct);

                return Ok(ApiResponse<IReadOnlyList<AgencyBranchDetailDto>>.Ok(
                    branches,
                    "Branches retrieved successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Agency not found"));
            }
        }

        // GET /api/agencies/{id}/branches/{branchId} - Get branch by ID
        [HttpGet("{id:guid}/branches/{branchId:guid}")]
        public async Task<IActionResult> GetBranch(Guid id, Guid branchId, CancellationToken ct)
        {
            var branch = await _agencyAppService.GetBranchByIdAsync(id, branchId, ct);

            if (branch == null)
                return NotFound(ApiResponse<object>.Fail("Branch not found"));

            return Ok(ApiResponse<AgencyBranchDetailDto>.Ok(
                branch,
                "Branch retrieved successfully"));
        }

        // PUT /api/agencies/{id}/branches/{branchId} - Update branch
        [HttpPut("{id:guid}/branches/{branchId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBranch(
            Guid id,
            Guid branchId,
            [FromBody] UpdateAgencyBranchDto dto,
            CancellationToken ct)
        {
            _logger.LogInformation("Updating branch: {BranchId} in agency: {AgencyId}", branchId, id);

            try
            {
                var branch = await _agencyAppService.UpdateBranchAsync(id, branchId, dto, ct);

                _logger.LogInformation("Branch updated successfully: {BranchId}", branchId);

                return Ok(ApiResponse<AgencyBranchDetailDto>.Ok(
                    branch,
                    "Branch updated successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Branch not found"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // DELETE /api/agencies/{id}/branches/{branchId} - Delete branch
        [HttpDelete("{id:guid}/branches/{branchId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteBranch(Guid id, Guid branchId, CancellationToken ct)
        {
            _logger.LogInformation("Deleting branch: {BranchId} from agency: {AgencyId}", branchId, id);

            try
            {
                await _agencyAppService.DeleteBranchAsync(id, branchId, ct);

                _logger.LogInformation("Branch deleted successfully: {BranchId}", branchId);

                return Ok(ApiResponse<object?>.Ok(null, "Branch deleted successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Branch not found"));
            }
        }
    }
}

