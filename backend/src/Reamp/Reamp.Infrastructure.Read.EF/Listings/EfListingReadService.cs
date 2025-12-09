using Microsoft.EntityFrameworkCore;
using Reamp.Application.Read.Listings;
using Reamp.Application.Read.Listings.DTOs;
using Reamp.Application.Read.Shared;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Listings.Enums;
using Reamp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Read.EF.Listings
{
    public sealed class EfListingReadService : IListingReadService
    {
        private readonly ApplicationDbContext _db;
        public EfListingReadService(ApplicationDbContext db) => _db = db;

        public async Task<PageResult<ListingSummaryDto>> ListAsync(
            Guid? agencyId = null,
            ListingStatus? status = null,
            ListingType? type = null,
            PropertyType? property = null,
            string? keyword = null,
            PageRequest? page = null,
            CancellationToken ct = default)
        {
            var q = _db.Set<Listing>().AsNoTracking().AsQueryable();

            if (agencyId.HasValue) q = q.Where(x => x.OwnerAgencyId == agencyId.Value);
            if (status.HasValue) q = q.Where(x => x.Status == status.Value);
            if (type.HasValue) q = q.Where(x => x.ListingType == type.Value);
            if (property.HasValue) q = q.Where(x => x.PropertyType == property.Value);
            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(x => x.Title.Contains(keyword!) || x.Description.Contains(keyword!));

            var p = page ?? new PageRequest();
            var pageNumber = p.Page <= 0 ? 1 : p.Page;
            var pageSize = p.PageSize <= 0 ? 20 : (p.PageSize > PageRequest.MaxPageSize ? PageRequest.MaxPageSize : p.PageSize);

            IOrderedQueryable<Listing> ordered = p.SortBy?.ToLowerInvariant() switch
            {
                "title" => p.Desc ? q.OrderByDescending(x => x.Title) : q.OrderBy(x => x.Title),
                "price" => p.Desc ? q.OrderByDescending(x => x.Price) : q.OrderBy(x => x.Price),
                _ => p.Desc ? q.OrderByDescending(x => x.Id) : q.OrderBy(x => x.Id),
            };

            var total = await ordered.CountAsync(ct);

            var items = await ordered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ListingSummaryDto(
                    x.Id, x.Title, x.Price, x.Currency, x.Status, x.ListingType, x.PropertyType))
                .ToListAsync(ct);

            return new PageResult<ListingSummaryDto>
            {
                Items = items,
                Total = total,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ListingDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
        {
            var q = _db.Set<Listing>().AsNoTracking().Where(l => l.Id == id);

            return await q
                .Select(l => new ListingDetailDto(
                    l.Id, l.Title, l.Description, l.Price, l.Currency,
                    l.Status, l.ListingType, l.PropertyType,
                    l.Bedrooms, l.Bathrooms, l.ParkingSpaces,
                    l.FloorAreaSqm, l.LandAreaSqm,
                    l.Address.Line1, l.Address.Line2, l.Address.City, l.Address.State, l.Address.Postcode, l.Address.Country,
                    l.MediaRefs.Where(m => m.IsVisible).OrderBy(m => m.SortOrder)
                        .Select(m => new ListingMediaItemDto(m.MediaAssetId, m.Role.ToString(), m.SortOrder, m.IsCover)).ToList(),
                    l.AgentSnapshots.OrderBy(a => a.SortOrder)
                        .Select(a => new ListingAgentItemDto(a.FirstName, a.LastName, a.Email, a.PhoneNumber, a.IsPrimary, a.SortOrder)).ToList()
                ))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<EditorDetailDto?> GetEditorDetailAsync(Guid id, CancellationToken ct = default)
        {
            var q = _db.Set<Listing>().AsNoTracking().Where(l => l.Id == id);

            return await q
                .Select(l => new EditorDetailDto(
                    l.Id, l.Title, l.Description, l.Price, l.Currency,
                    l.Status, l.ListingType, l.PropertyType,
                    l.Bedrooms, l.Bathrooms, l.ParkingSpaces,
                    l.FloorAreaSqm, l.LandAreaSqm,
                    l.Address.Line1, l.Address.Line2, l.Address.City, l.Address.State, l.Address.Postcode, l.Address.Country,
                    l.MediaRefs.OrderBy(m => m.SortOrder)
                        .Select(m => new EditorMediaItemDto(m.Id, m.MediaAssetId, m.Role.ToString(), m.SortOrder, m.IsCover, m.IsVisible)).ToList(),
                    l.AgentSnapshots.OrderBy(a => a.SortOrder)
                        .Select(a => new EditorAgentItemDto(a.Id, a.FirstName, a.LastName, a.Email, a.PhoneNumber, a.IsPrimary, a.SortOrder)).ToList()
                ))
                .FirstOrDefaultAsync(ct);
        }
    }
}