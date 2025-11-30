using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Accounts.Clients.Dtos;
using Reamp.Application.Accounts.Clients.Services;
using Reamp.Application.Read.Clients;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Common.Abstractions;
using Reamp.Shared;
using PageRequest = Reamp.Application.Read.Shared.PageRequest;

namespace Reamp.Api.Controllers.Accounts
{
    [ApiController]
    [Route("api")]
    public sealed class ClientsController : ControllerBase
    {
        private readonly IClientAppService _clientAppService;
        private readonly IClientReadService _clientReadService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            IClientAppService clientAppService,
            IClientReadService clientReadService,
            ILogger<ClientsController> logger)
        {
            _clientAppService = clientAppService;
            _clientReadService = clientReadService;
            _logger = logger;
        }

        // POST /api/agencies/{agencyId}/clients - Add client
        [HttpPost("agencies/{agencyId:guid}/clients")]
        [Authorize]
        public async Task<IActionResult> Create(Guid agencyId, [FromBody] CreateClientDto dto, CancellationToken ct)
        {
            // Ensure DTO AgencyId matches route parameter
            if (dto.AgencyId != agencyId)
                return BadRequest(ApiResponse<object>.Fail("AgencyId in route and body must match."));

            _logger.LogInformation("Creating client for agency: {AgencyId}", agencyId);

            try
            {
                var client = await _clientAppService.CreateAsync(dto, ct);

                _logger.LogInformation("Client created successfully: {ClientId}", client.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = client.Id },
                    ApiResponse<ClientDetailDto>.Ok(client, "Client created successfully"));
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

        // GET /api/agencies/{agencyId}/clients - List clients
        [HttpGet("agencies/{agencyId:guid}/clients")]
        public async Task<IActionResult> ListByAgency(
            Guid agencyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
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
                var result = await _clientReadService.ListByAgencyAsync(agencyId, search, pageRequest, ct);

                return Ok(ApiResponse<Reamp.Application.Read.Shared.PageResult<Application.Read.Clients.DTOs.ClientSummaryDto>>.Ok(
                    result,
                    "Clients retrieved successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Agency not found"));
            }
        }

        // GET /api/clients/{id} - Get client details
        [HttpGet("clients/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var client = await _clientAppService.GetByIdAsync(id, ct);

            if (client == null)
                return NotFound(ApiResponse<object>.Fail("Client not found"));

            return Ok(ApiResponse<ClientDetailDto>.Ok(client, "Client retrieved successfully"));
        }

        // PUT /api/clients/{id} - Update client
        [HttpPut("clients/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating client: {ClientId}", id);

            try
            {
                var client = await _clientAppService.UpdateAsync(id, dto, ct);

                _logger.LogInformation("Client updated successfully: {ClientId}", id);

                return Ok(ApiResponse<ClientDetailDto>.Ok(client, "Client updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }

        // DELETE /api/clients/{id} - Remove client
        [HttpDelete("clients/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Deleting client: {ClientId}", id);

            try
            {
                await _clientAppService.DeleteAsync(id, ct);

                _logger.LogInformation("Client deleted successfully: {ClientId}", id);

                return Ok(ApiResponse<object?>.Ok(null, "Client deleted successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Client not found"));
            }
        }
    }
}

