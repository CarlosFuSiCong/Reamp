using Microsoft.EntityFrameworkCore;
using Reamp.Application.Read.Clients;
using Reamp.Application.Read.Clients.DTOs;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Accounts.Entities;
using Reamp.Infrastructure;

namespace Reamp.Infrastructure.Read.EF.Clients
{
    public sealed class EfClientReadService : IClientReadService
    {
        private readonly ApplicationDbContext _dbContext;

        public EfClientReadService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PageResult<ClientSummaryDto>> ListByAgencyAsync(
            Guid agencyId,
            string? search,
            PageRequest pageRequest,
            CancellationToken ct = default)
        {
            var query = _dbContext.Set<Client>()
                .AsNoTracking()
                .Where(c => c.AgencyId == agencyId && c.DeletedAtUtc == null);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(c =>
                    _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Any(u => u.FirstName.ToLower().Contains(searchLower) ||
                                u.LastName.ToLower().Contains(searchLower))
                );
            }

            // Normalize page request
            var pageNumber = pageRequest.Page <= 0 ? 1 : pageRequest.Page;
            var pageSize = pageRequest.PageSize <= 0 ? 20 : (pageRequest.PageSize > PageRequest.MaxPageSize ? PageRequest.MaxPageSize : pageRequest.PageSize);

            var totalCount = await query.CountAsync(ct);

            // Apply sorting and pagination
            var sortBy = pageRequest.SortBy?.ToLower();
            query = sortBy switch
            {
                "createdat" => pageRequest.Desc 
                    ? query.OrderByDescending(c => c.CreatedAtUtc)
                    : query.OrderBy(c => c.CreatedAtUtc),
                _ => query.OrderByDescending(c => c.CreatedAtUtc) // default sort
            };

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClientSummaryDto
                {
                    Id = c.Id,
                    UserProfileId = c.UserProfileId,
                    FirstName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "",
                    LastName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.LastName)
                        .FirstOrDefault() ?? "",
                    DisplayName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "",
                    AgencyId = c.AgencyId,
                    AgencyName = _dbContext.Set<Agency>()
                        .Where(a => a.Id == c.AgencyId)
                        .Select(a => a.Name)
                        .FirstOrDefault(),
                    AgencyBranchId = c.AgencyBranchId,
                    AgencyBranchName = c.AgencyBranchId.HasValue
                        ? _dbContext.Set<AgencyBranch>()
                            .Where(b => b.Id == c.AgencyBranchId)
                            .Select(b => b.Name)
                            .FirstOrDefault()
                        : null,
                    CreatedAtUtc = c.CreatedAtUtc
                })
                .ToListAsync(ct);

            return new PageResult<ClientSummaryDto>
            {
                Items = items,
                Total = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ClientSummaryDto?> GetByIdAsync(Guid clientId, CancellationToken ct = default)
        {
            return await _dbContext.Set<Client>()
                .AsNoTracking()
                .Where(c => c.Id == clientId && c.DeletedAtUtc == null)
                .Select(c => new ClientSummaryDto
                {
                    Id = c.Id,
                    UserProfileId = c.UserProfileId,
                    FirstName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "",
                    LastName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.LastName)
                        .FirstOrDefault() ?? "",
                    DisplayName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "",
                    AgencyId = c.AgencyId,
                    AgencyName = _dbContext.Set<Agency>()
                        .Where(a => a.Id == c.AgencyId)
                        .Select(a => a.Name)
                        .FirstOrDefault(),
                    AgencyBranchId = c.AgencyBranchId,
                    AgencyBranchName = c.AgencyBranchId.HasValue
                        ? _dbContext.Set<AgencyBranch>()
                            .Where(b => b.Id == c.AgencyBranchId)
                            .Select(b => b.Name)
                            .FirstOrDefault()
                        : null,
                    CreatedAtUtc = c.CreatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ClientSummaryDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            return await _dbContext.Set<Client>()
                .AsNoTracking()
                .Where(c => c.UserProfileId == userProfileId && c.DeletedAtUtc == null)
                .Select(c => new ClientSummaryDto
                {
                    Id = c.Id,
                    UserProfileId = c.UserProfileId,
                    FirstName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "",
                    LastName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.LastName)
                        .FirstOrDefault() ?? "",
                    DisplayName = _dbContext.Set<UserProfile>()
                        .Where(u => u.Id == c.UserProfileId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "",
                    AgencyId = c.AgencyId,
                    AgencyName = _dbContext.Set<Agency>()
                        .Where(a => a.Id == c.AgencyId)
                        .Select(a => a.Name)
                        .FirstOrDefault(),
                    AgencyBranchId = c.AgencyBranchId,
                    AgencyBranchName = c.AgencyBranchId.HasValue
                        ? _dbContext.Set<AgencyBranch>()
                            .Where(b => b.Id == c.AgencyBranchId)
                            .Select(b => b.Name)
                            .FirstOrDefault()
                        : null,
                    CreatedAtUtc = c.CreatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}

