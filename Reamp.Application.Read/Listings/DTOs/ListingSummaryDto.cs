using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Read.Listings.DTOs
{
    public sealed record ListingSummaryDto(
        Guid Id, string Title, decimal Price, string Currency,
        ListingStatus Status, ListingType ListingType, PropertyType PropertyType);
}