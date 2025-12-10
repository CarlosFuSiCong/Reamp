using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Authentication;
using Reamp.Application.Delivery.Dtos;
using Reamp.Application.Delivery.Services;
using Reamp.Shared;

namespace Reamp.Api.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    [Authorize]
    public sealed class DeliveryController : ControllerBase
    {
        private readonly IDeliveryPackageAppService _appService;
        private readonly ILogger<DeliveryController> _logger;

        public DeliveryController(
            IDeliveryPackageAppService appService,
            ILogger<DeliveryController> logger)
        {
            _appService = appService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> CreatePackage(
            [FromBody] CreateDeliveryPackageDto dto,
            CancellationToken ct)
        {
            var result = await _appService.CreateAsync(dto, ct);
            _logger.LogInformation("Delivery package created: {PackageId}", result.Id);

            return CreatedAtAction(
                nameof(GetPackageDetail),
                new { id = result.Id },
                ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package created successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageDetail(Guid id, CancellationToken ct)
        {
            var result = await _appService.GetByIdAsync(id, ct);
            if (result == null)
                return NotFound(ApiResponse<object>.Fail("Delivery package not found"));

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result));
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPackagesByOrder(Guid orderId, CancellationToken ct)
        {
            var result = await _appService.GetByOrderIdAsync(orderId, ct);
            return Ok(ApiResponse<List<DeliveryPackageListDto>>.Ok(result));
        }

        [HttpGet("listing/{listingId}")]
        public async Task<IActionResult> GetPackagesByListing(Guid listingId, CancellationToken ct)
        {
            var result = await _appService.GetByListingIdAsync(listingId, ct);
            return Ok(ApiResponse<List<DeliveryPackageListDto>>.Ok(result));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> UpdatePackage(
            Guid id,
            [FromBody] UpdateDeliveryPackageDto dto,
            CancellationToken ct)
        {
            var result = await _appService.UpdateAsync(id, dto, ct);
            _logger.LogInformation("Delivery package updated: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> DeletePackage(Guid id, CancellationToken ct)
        {
            await _appService.DeleteAsync(id, ct);
            _logger.LogInformation("Delivery package deleted: {PackageId}", id);

            return Ok(ApiResponse.Ok("Delivery package deleted successfully"));
        }

        [HttpPost("{id}/items")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> AddItem(
            Guid id,
            [FromBody] AddDeliveryItemDto dto,
            CancellationToken ct)
        {
            var result = await _appService.AddItemAsync(id, dto, ct);
            _logger.LogInformation("Item added to delivery package: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Item added successfully"));
        }

        [HttpDelete("{id}/items/{itemId}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> RemoveItem(Guid id, Guid itemId, CancellationToken ct)
        {
            var result = await _appService.RemoveItemAsync(id, itemId, ct);
            _logger.LogInformation("Item removed from delivery package: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Item removed successfully"));
        }

        [HttpPost("{id}/accesses")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> AddAccess(
            Guid id,
            [FromBody] AddDeliveryAccessDto dto,
            CancellationToken ct)
        {
            var result = await _appService.AddAccessAsync(id, dto, ct);
            _logger.LogInformation("Access added to delivery package: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Access added successfully"));
        }

        [HttpDelete("{id}/accesses/{accessId}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> RemoveAccess(Guid id, Guid accessId, CancellationToken ct)
        {
            var result = await _appService.RemoveAccessAsync(id, accessId, ct);
            _logger.LogInformation("Access removed from delivery package: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Access removed successfully"));
        }

        [HttpPost("{id}/publish")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> PublishPackage(Guid id, CancellationToken ct)
        {
            var result = await _appService.PublishAsync(id, ct);
            _logger.LogInformation("Delivery package published: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package published successfully"));
        }

        [HttpPost("{id}/revoke")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> RevokePackage(Guid id, CancellationToken ct)
        {
            var result = await _appService.RevokeAsync(id, ct);
            _logger.LogInformation("Delivery package revoked: {PackageId}", id);

            return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package revoked successfully"));
        }

        [HttpPost("{id}/verify-password")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPassword(
            Guid id,
            [FromBody] VerifyPasswordDto dto,
            CancellationToken ct)
        {
            var isValid = await _appService.VerifyAccessPasswordAsync(id, dto.Password, ct);
            
            if (isValid)
                return Ok(ApiResponse<bool>.Ok(true, "Password verified"));
            
            return Unauthorized(ApiResponse<bool>.Fail("Invalid password"));
        }

        [HttpPost("{id}/download/{accessId}")]
        [AllowAnonymous]
        public async Task<IActionResult> IncrementDownload(Guid id, Guid accessId, CancellationToken ct)
        {
            await _appService.IncrementDownloadAsync(id, accessId, ct);
            _logger.LogInformation("Download incremented for delivery package: {PackageId}, Access: {AccessId}", id, accessId);

            return Ok(ApiResponse.Ok("Download recorded"));
        }
    }

    public sealed class VerifyPasswordDto
    {
        public string Password { get; set; } = default!;
    }
}
