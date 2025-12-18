using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Read.Listings.DTOs
{
    public sealed record ListingDetailDto(
        Guid Id, Guid OwnerAgencyId, string Title, string Description, decimal Price, string Currency,
        ListingStatus Status, ListingType ListingType, PropertyType PropertyType,
        int Bedrooms, int Bathrooms, int ParkingSpaces,
        double? FloorAreaSqm, double? LandAreaSqm,
        string AddressLine1, string? AddressLine2, string City, string State, string Postcode, string Country,
        double? Latitude, double? Longitude,
        IReadOnlyList<ListingMediaItemDto> Media, IReadOnlyList<ListingAgentItemDto> Agents);

    public sealed record ListingMediaItemDto(Guid MediaAssetId, string Role, int SortOrder, bool IsCover, string? ThumbnailUrl, bool IsVisible = true);
    public sealed record ListingAgentItemDto(string FirstName, string LastName, string Email, string? Phone, bool IsPrimary, int SortOrder);
}