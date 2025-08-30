using Microsoft.AspNetCore.Mvc;
using Reamp.Application.Accounts.Studios.Dtos;
using Reamp.Application.Accounts.Studios.Services;

namespace Reamp.Api.Controllers.Accounts
{
    [ApiController]
    [Route("api/accounts/studios")]
    public sealed class StudiosController : ControllerBase
    {
        private readonly IStudioAppService _svc;
        public StudiosController(IStudioAppService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudioDto dto, CancellationToken ct)
        {
            var id = await _svc.CreateAsync(dto, ct);
            var slug = Slugify(dto.Name);
            return CreatedAtAction(nameof(GetBySlug), new { slug }, new { id });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? q = null, CancellationToken ct = default)
            => Ok(await _svc.ListAsync(page, pageSize, q, ct));

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug([FromRoute] string slug, CancellationToken ct)
        {
            var studio = await _svc.GetBySlugAsync(slug, ct);
            return studio is null ? NotFound() : Ok(studio);
        }

        private static string Slugify(string name) =>
            string.IsNullOrWhiteSpace(name)
                ? string.Empty
                : System.Text.RegularExpressions.Regex
                    .Replace(name.Trim().ToLowerInvariant(), @"[^a-z0-9\-]+", "-")
                    .Trim('-');
    }
}