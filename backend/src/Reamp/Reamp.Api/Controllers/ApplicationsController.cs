using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Applications.Dtos;
using Reamp.Application.Applications.Services;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Shared;
using System;
using System.Security.Claims;
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
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(
            IApplicationService applicationService,
            IUserProfileRepository userProfileRepo,
            ILogger<ApplicationsController> logger)
        {
            _applicationService = applicationService;
            _userProfileRepo = userProfileRepo;
            _logger = logger;
        }

        private async Task<Guid?> GetUserProfileIdAsync(CancellationToken ct)
        {
            var applicationUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (applicationUserIdClaim == null || !Guid.TryParse(applicationUserIdClaim, out var applicationUserId))
                return null;

            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(applicationUserId, includeDeleted: false, asNoTracking: true, ct);
            return profile?.Id;
        }

        [HttpPost("agency")]
        public async Task<IActionResult> SubmitAgencyApplication(
            [FromBody] SubmitAgencyApplicationDto dto,
            CancellationToken ct)
        {
            var userProfileId = await GetUserProfileIdAsync(ct);
            if (userProfileId == null)
                return Unauthorized(ApiResponse.Fail("User profile not found"));

            var applicationId = await _applicationService.SubmitAgencyApplicationAsync(userProfileId.Value, dto, ct);

            _logger.LogInformation("User {UserId} submitted agency application {ApplicationId}", userProfileId, applicationId);
            return Ok(ApiResponse<Guid>.Ok(applicationId, "Agency application submitted successfully"));
        }

        [HttpPost("studio")]
        public async Task<IActionResult> SubmitStudioApplication(
            [FromBody] SubmitStudioApplicationDto dto,
            CancellationToken ct)
        {
            var userProfileId = await GetUserProfileIdAsync(ct);
            if (userProfileId == null)
                return Unauthorized(ApiResponse.Fail("User profile not found"));

            var applicationId = await _applicationService.SubmitStudioApplicationAsync(userProfileId.Value, dto, ct);

            _logger.LogInformation("User {UserId} submitted studio application {ApplicationId}", userProfileId, applicationId);
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
            return Ok(ApiResponse<IPagedList<ApplicationListDto>>.Ok(result));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyApplications(CancellationToken ct)
        {
            var userProfileId = await GetUserProfileIdAsync(ct);
            if (userProfileId == null)
                return Unauthorized(ApiResponse.Fail("User profile not found"));

            var applications = await _applicationService.GetMyApplicationsAsync(userProfileId.Value, ct);

            return Ok(ApiResponse<List<ApplicationListDto>>.Ok(applications));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationDetail(Guid id, CancellationToken ct)
        {
            var userProfileId = await GetUserProfileIdAsync(ct);
            if (userProfileId == null)
                return Unauthorized(ApiResponse.Fail("User profile not found"));

            var application = await _applicationService.GetApplicationDetailAsync(id, ct);

            var isAdmin = User.IsInRole(nameof(UserRole.Admin));
            if (!isAdmin && application.ApplicantUserId != userProfileId)
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
            var userProfileId = await GetUserProfileIdAsync(ct);
            if (userProfileId == null)
                return Unauthorized(ApiResponse.Fail("User profile not found"));

            await _applicationService.ReviewApplicationAsync(id, userProfileId.Value, dto, ct);

            var action = dto.Approved ? "approved" : "rejected";
            _logger.LogInformation("Admin {AdminId} {Action} application {ApplicationId}", userProfileId, action, id);
            
            return Ok(ApiResponse.Ok($"Application {action} successfully"));
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelApplication(Guid id, CancellationToken ct)
        {
            var userProfileId = await GetUserProfileIdAsync(ct);
            if (userProfileId == null)
                return Unauthorized(ApiResponse.Fail("User profile not found"));

            await _applicationService.CancelApplicationAsync(id, userProfileId.Value, ct);

            _logger.LogInformation("User {UserId} cancelled application {ApplicationId}", userProfileId, id);
            return Ok(ApiResponse.Ok("Application cancelled successfully"));
        }
    }
}
