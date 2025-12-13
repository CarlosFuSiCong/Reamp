using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Applications.Dtos;
using Reamp.Application.Applications.Services;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Common.Abstractions;
using Reamp.Shared;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/applications")]
    [Authorize]
    public sealed class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(
            IApplicationService applicationService,
            ILogger<ApplicationsController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpPost("agency")]
        public async Task<IActionResult> SubmitAgencyApplication(
            [FromBody] SubmitAgencyApplicationDto dto,
            CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse.Fail("Invalid token"));

            var applicationId = await _applicationService.SubmitAgencyApplicationAsync(userId, dto, ct);

            _logger.LogInformation("User {UserId} submitted agency application {ApplicationId}", userId, applicationId);
            return Ok(ApiResponse<Guid>.Ok(applicationId, "Agency application submitted successfully"));
        }

        [HttpPost("studio")]
        public async Task<IActionResult> SubmitStudioApplication(
            [FromBody] SubmitStudioApplicationDto dto,
            CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse.Fail("Invalid token"));

            var applicationId = await _applicationService.SubmitStudioApplicationAsync(userId, dto, ct);

            _logger.LogInformation("User {UserId} submitted studio application {ApplicationId}", userId, applicationId);
            return Ok(ApiResponse<Guid>.Ok(applicationId, "Studio application submitted successfully"));
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetApplications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] ApplicationStatus? status = null,
            [FromQuery] ApplicationType? type = null,
            CancellationToken ct = default)
        {
            var pageRequest = new PageRequest(page, pageSize);
            var result = await _applicationService.GetApplicationsAsync(pageRequest, status, type, ct);

            _logger.LogInformation("Admin retrieved applications page {Page}", page);
            return Ok(ApiResponse<PagedResult<ApplicationListDto>>.Ok(result));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyApplications(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse.Fail("Invalid token"));

            var applications = await _applicationService.GetMyApplicationsAsync(userId, ct);

            return Ok(ApiResponse<List<ApplicationListDto>>.Ok(applications));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationDetail(Guid id, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse.Fail("Invalid token"));

            var application = await _applicationService.GetApplicationDetailAsync(id, ct);

            var isAdmin = User.IsInRole(nameof(UserRole.Admin));
            if (!isAdmin && application.ApplicantUserId != userId)
                return Forbid();

            return Ok(ApiResponse<ApplicationDetailDto>.Ok(application));
        }

        [HttpPost("{id}/review")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> ReviewApplication(
            Guid id,
            [FromBody] ReviewApplicationDto dto,
            CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse.Fail("Invalid token"));

            await _applicationService.ReviewApplicationAsync(id, userId, dto, ct);

            var action = dto.Approved ? "approved" : "rejected";
            _logger.LogInformation("Admin {AdminId} {Action} application {ApplicationId}", userId, action, id);
            
            return Ok(ApiResponse.Ok($"Application {action} successfully"));
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelApplication(Guid id, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse.Fail("Invalid token"));

            await _applicationService.CancelApplicationAsync(id, userId, ct);

            _logger.LogInformation("User {UserId} cancelled application {ApplicationId}", userId, id);
            return Ok(ApiResponse.Ok("Application cancelled successfully"));
        }
    }
}
