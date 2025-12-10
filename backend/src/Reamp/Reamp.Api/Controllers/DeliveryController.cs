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

        // POST /api/delivery - Create delivery package
        [HttpPost]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> CreatePackage(
            [FromBody] CreateDeliveryPackageDto dto,
            CancellationToken ct)
        {
            try
            {
                var result = await _appService.CreateAsync(dto, ct);
                _logger.LogInformation("Delivery package created: {PackageId}", result.Id);

                return CreatedAtAction(
                    nameof(GetPackageDetail),
                    new { id = result.Id },
                    ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package created successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating delivery package");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating delivery package");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/delivery/{id} - Get delivery package detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageDetail(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _appService.GetByIdAsync(id, ct);
                if (result == null)
                    return NotFound(ApiResponse<object>.Fail("Delivery package not found"));

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/delivery/order/{orderId} - Get delivery packages by order
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPackagesByOrder(Guid orderId, CancellationToken ct)
        {
            try
            {
                var result = await _appService.GetByOrderIdAsync(orderId, ct);
                return Ok(ApiResponse<List<DeliveryPackageListDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting delivery packages for order: {OrderId}", orderId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // GET /api/delivery/listing/{listingId} - Get delivery packages by listing
        [HttpGet("listing/{listingId}")]
        public async Task<IActionResult> GetPackagesByListing(Guid listingId, CancellationToken ct)
        {
            try
            {
                var result = await _appService.GetByListingIdAsync(listingId, ct);
                return Ok(ApiResponse<List<DeliveryPackageListDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting delivery packages for listing: {ListingId}", listingId);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // PUT /api/delivery/{id} - Update delivery package
        [HttpPut("{id}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> UpdatePackage(
            Guid id,
            [FromBody] UpdateDeliveryPackageDto dto,
            CancellationToken ct)
        {
            try
            {
                var result = await _appService.UpdateAsync(id, dto, ct);
                _logger.LogInformation("Delivery package updated: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when updating delivery package");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // DELETE /api/delivery/{id} - Delete delivery package
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> DeletePackage(Guid id, CancellationToken ct)
        {
            try
            {
                await _appService.DeleteAsync(id, ct);
                _logger.LogInformation("Delivery package deleted: {PackageId}", id);

                return Ok(ApiResponse.Ok("Delivery package deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/delivery/{id}/items - Add item to delivery package
        [HttpPost("{id}/items")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> AddItem(
            Guid id,
            [FromBody] AddDeliveryItemDto dto,
            CancellationToken ct)
        {
            try
            {
                var result = await _appService.AddItemAsync(id, dto, ct);
                _logger.LogInformation("Item added to delivery package: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Item added successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when adding item");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // DELETE /api/delivery/{id}/items/{itemId} - Remove item from delivery package
        [HttpDelete("{id}/items/{itemId}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> RemoveItem(Guid id, Guid itemId, CancellationToken ct)
        {
            try
            {
                var result = await _appService.RemoveItemAsync(id, itemId, ct);
                _logger.LogInformation("Item removed from delivery package: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Item removed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/delivery/{id}/accesses - Add access control
        [HttpPost("{id}/accesses")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> AddAccess(
            Guid id,
            [FromBody] AddDeliveryAccessDto dto,
            CancellationToken ct)
        {
            try
            {
                var result = await _appService.AddAccessAsync(id, dto, ct);
                _logger.LogInformation("Access added to delivery package: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Access added successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when adding access");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding access to delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // DELETE /api/delivery/{id}/accesses/{accessId} - Remove access control
        [HttpDelete("{id}/accesses/{accessId}")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> RemoveAccess(Guid id, Guid accessId, CancellationToken ct)
        {
            try
            {
                var result = await _appService.RemoveAccessAsync(id, accessId, ct);
                _logger.LogInformation("Access removed from delivery package: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Access removed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing access from delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/delivery/{id}/publish - Publish delivery package
        [HttpPost("{id}/publish")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> PublishPackage(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _appService.PublishAsync(id, ct);
                _logger.LogInformation("Delivery package published: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package published successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when publishing delivery package");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/delivery/{id}/revoke - Revoke published delivery package
        [HttpPost("{id}/revoke")]
        [Authorize(Policy = AuthPolicies.RequireStaffOrAdmin)]
        public async Task<IActionResult> RevokePackage(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _appService.RevokeAsync(id, ct);
                _logger.LogInformation("Delivery package revoked: {PackageId}", id);

                return Ok(ApiResponse<DeliveryPackageDetailDto>.Ok(result, "Delivery package revoked successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when revoking delivery package");
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/delivery/{id}/verify-password - Verify access password (public endpoint)
        [HttpPost("{id}/verify-password")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPassword(
            Guid id,
            [FromBody] VerifyPasswordDto dto,
            CancellationToken ct)
        {
            try
            {
                var isValid = await _appService.VerifyAccessPasswordAsync(id, dto.Password, ct);
                
                if (isValid)
                {
                    return Ok(ApiResponse<bool>.Ok(true, "Password verified"));
                }
                
                return Unauthorized(ApiResponse<bool>.Fail("Invalid password"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password for delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }

        // POST /api/delivery/{id}/download/{accessId} - Increment download counter (public endpoint)
        [HttpPost("{id}/download/{accessId}")]
        [AllowAnonymous]
        public async Task<IActionResult> IncrementDownload(Guid id, Guid accessId, CancellationToken ct)
        {
            try
            {
                await _appService.IncrementDownloadAsync(id, accessId, ct);
                _logger.LogInformation("Download incremented for delivery package: {PackageId}, Access: {AccessId}", id, accessId);

                return Ok(ApiResponse.Ok("Download recorded"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing download for delivery package: {PackageId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred"));
            }
        }
    }

    // DTO for password verification
    public sealed class VerifyPasswordDto
    {
        public string Password { get; set; } = default!;
    }
}

