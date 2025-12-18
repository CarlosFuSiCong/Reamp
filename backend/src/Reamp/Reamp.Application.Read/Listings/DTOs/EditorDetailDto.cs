using Reamp.Domain.Listings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Application.Read.Listings.DTOs
{
    public sealed record EditorDetailDto(
        Guid Id, string Title, string Description, decimal Price, string Currency,
        ListingStatus Status, ListingType ListingType, PropertyType PropertyType,
        int Bedrooms, int Bathrooms, int ParkingSpaces,
        double? FloorAreaSqm, double? LandAreaSqm,
        string AddressLine1, string? AddressLine2, string City, string State, string Postcode, string Country,
        double? Latitude, double? Longitude,
        IReadOnlyList<EditorMediaItemDto> Media, IReadOnlyList<EditorAgentItemDto> Agents);

    public sealed record EditorMediaItemDto(Guid Id, Guid MediaAssetId, string Role, int SortOrder, bool IsCover, bool IsVisible, string? ThumbnailUrl);
    public sealed record EditorAgentItemDto(Guid Id, string FirstName, string LastName, string Email, string? Phone, bool IsPrimary, int SortOrder);
}