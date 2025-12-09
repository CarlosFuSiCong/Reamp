using Reamp.Application.Read.Shared;
using Reamp.Application.Read.Listings.DTOs;
using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Read.Listings
{
    public interface IListingReadService
    {
        Task<PageResult<ListingSummaryDto>> ListAsync(
            Guid? agencyId = null,
            ListingStatus? status = null,
            ListingType? type = null,
            PropertyType? property = null,
            string? keyword = null,
            PageRequest? page = null,
            CancellationToken ct = default);

        Task<ListingDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
        Task<EditorDetailDto?> GetEditorDetailAsync(Guid id, CancellationToken ct = default);
    }
}