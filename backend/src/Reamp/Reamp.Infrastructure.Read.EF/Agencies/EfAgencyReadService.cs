using Microsoft.EntityFrameworkCore;
using Reamp.Application.Read.Agencies;
using Reamp.Application.Read.Agencies.DTOs;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Accounts.Entities;
using Reamp.Infrastructure;

namespace Reamp.Infrastructure.Read.EF.Agencies
{
    public sealed class EfAgencyReadService : IAgencyReadService
    {
        private readonly ApplicationDbContext _db;

        public EfAgencyReadService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PageResult<AgencySummaryDto>> ListAsync(
            string? search = null,
            PageRequest? page = null,
            CancellationToken ct = default)
        {
            var query = _db.Set<Agency>()
                .AsNoTracking()
                .Where(a => a.DeletedAtUtc == null);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLower();
                query = query.Where(a =>
                    a.Name.ToLower().Contains(searchTerm) ||
                    a.Slug.Value.Contains(searchTerm) ||
                    (a.Description != null && a.Description.ToLower().Contains(searchTerm))
                );
            }

            // Normalize page request
            var p = page ?? new PageRequest();
            var pageNumber = p.Page <= 0 ? 1 : p.Page;
            var pageSize = p.PageSize <= 0 ? 20 : (p.PageSize > PageRequest.MaxPageSize ? PageRequest.MaxPageSize : p.PageSize);

            // Apply sorting
            IOrderedQueryable<Agency> ordered = p.SortBy?.ToLowerInvariant() switch
            {
                "name" => p.Desc ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name),
                "created" => p.Desc ? query.OrderByDescending(a => a.CreatedAtUtc) : query.OrderBy(a => a.CreatedAtUtc),
                _ => query.OrderBy(a => a.Name)
            };

            // Get total count
            var total = await ordered.CountAsync(ct);

            // Get paginated data with projection
            var items = await ordered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AgencySummaryDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Slug = a.Slug.Value,
                    Description = a.Description,
                    LogoAssetId = a.LogoAssetId,
                    LogoUrl = null, // TODO: Map from MediaAsset when needed
                    ContactEmail = a.ContactEmail,
                    ContactPhone = a.ContactPhone,
                    BranchCount = a.Branches.Count(b => b.DeletedAtUtc == null),
                    CreatedAtUtc = a.CreatedAtUtc
                })
                .ToListAsync(ct);

            return new PageResult<AgencySummaryDto>
            {
                Items = items,
                Total = total,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<AgencyDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Set<Agency>()
                .AsNoTracking()
                .Where(a => a.Id == id && a.DeletedAtUtc == null)
                .Select(a => new AgencyDetailDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Slug = a.Slug.Value,
                    Description = a.Description,
                    LogoAssetId = a.LogoAssetId,
                    LogoUrl = null, // TODO: Map from MediaAsset when needed
                    ContactEmail = a.ContactEmail,
                    ContactPhone = a.ContactPhone,
                    CreatedBy = a.CreatedBy,
                    CreatedAtUtc = a.CreatedAtUtc,
                    UpdatedAtUtc = a.UpdatedAtUtc,
                    BranchCount = a.Branches.Count(b => b.DeletedAtUtc == null),
                    Branches = a.Branches
                        .Where(b => b.DeletedAtUtc == null)
                        .OrderBy(b => b.Name)
                        .Select(b => new AgencyBranchSummaryDto
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Slug = b.Slug.Value,
                            Description = b.Description,
                            ContactEmail = b.ContactEmail,
                            ContactPhone = b.ContactPhone,
                            Address = b.Address != null ? new AddressDto
                            {
                                Line1 = b.Address.Line1,
                                Line2 = b.Address.Line2,
                                City = b.Address.City,
                                State = b.Address.State,
                                Postcode = b.Address.Postcode,
                                Country = b.Address.Country,
                                Latitude = b.Address.Latitude,
                                Longitude = b.Address.Longitude
                            } : null,
                            CreatedAtUtc = b.CreatedAtUtc
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<AgencyDetailDto?> GetDetailBySlugAsync(string slug, CancellationToken ct = default)
        {
            var slugLower = slug.Trim().ToLower();
            
            return await _db.Set<Agency>()
                .AsNoTracking()
                .Where(a => a.Slug.Value == slugLower && a.DeletedAtUtc == null)
                .Select(a => new AgencyDetailDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Slug = a.Slug.Value,
                    Description = a.Description,
                    LogoAssetId = a.LogoAssetId,
                    LogoUrl = null, // TODO: Map from MediaAsset when needed
                    ContactEmail = a.ContactEmail,
                    ContactPhone = a.ContactPhone,
                    CreatedBy = a.CreatedBy,
                    CreatedAtUtc = a.CreatedAtUtc,
                    UpdatedAtUtc = a.UpdatedAtUtc,
                    BranchCount = a.Branches.Count(b => b.DeletedAtUtc == null),
                    Branches = a.Branches
                        .Where(b => b.DeletedAtUtc == null)
                        .OrderBy(b => b.Name)
                        .Select(b => new AgencyBranchSummaryDto
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Slug = b.Slug.Value,
                            Description = b.Description,
                            ContactEmail = b.ContactEmail,
                            ContactPhone = b.ContactPhone,
                            Address = b.Address != null ? new AddressDto
                            {
                                Line1 = b.Address.Line1,
                                Line2 = b.Address.Line2,
                                City = b.Address.City,
                                State = b.Address.State,
                                Postcode = b.Address.Postcode,
                                Country = b.Address.Country,
                                Latitude = b.Address.Latitude,
                                Longitude = b.Address.Longitude
                            } : null,
                            CreatedAtUtc = b.CreatedAtUtc
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        {
            var slugLower = slug.Trim().ToLower();
            return await _db.Set<Agency>()
                .AsNoTracking()
                .AnyAsync(a => a.Slug.Value == slugLower && a.DeletedAtUtc == null, ct);
        }
    }
}



