using FluentAssertions;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Listings.Enums;
using Reamp.Domain.Common.ValueObjects;

namespace Reamp.Tests.Unit.Domain.Listings;

public class ListingTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateListing()
    {
        var title = "Modern Apartment";
        var description = "Beautiful 2BR apartment";
        var price = 500000m;
        var address = new Address("123 Main St", "Melbourne", "VIC", "3000", "AU");
        var agencyId = Guid.NewGuid();

        var listing = new Listing(
            agencyId,
            title,
            description,
            price,
            ListingType.ForSale,
            PropertyType.Apartment,
            address);

        listing.Should().NotBeNull();
        listing.Title.Should().Be(title);
        listing.Description.Should().Be(description);
        listing.Price.Should().Be(price);
        listing.PropertyType.Should().Be(PropertyType.Apartment);
        listing.OwnerAgencyId.Should().Be(agencyId);
        listing.Status.Should().Be(ListingStatus.Draft);
        listing.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        var listing = CreateTestListing();

        listing.SoftDelete();

        listing.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Restore_WhenDeleted_ShouldUnmarkDeleted()
    {
        var listing = CreateTestListing();
        listing.SoftDelete();

        listing.Restore();

        listing.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void AddMedia_ShouldAddMediaReference()
    {
        var listing = CreateTestListing();
        var mediaId = Guid.NewGuid();
        var role = ListingMediaRole.Gallery;

        listing.AddMedia(mediaId, role);

        listing.MediaRefs.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveMedia_ShouldRemoveMediaReference()
    {
        var listing = CreateTestListing();
        var mediaId = Guid.NewGuid();
        var role = ListingMediaRole.Gallery;
        listing.AddMedia(mediaId, role);
        var refId = listing.MediaRefs.First().Id;

        listing.RemoveMedia(refId);

        listing.MediaRefs.Should().BeEmpty();
    }

    private static Listing CreateTestListing()
    {
        var address = new Address("123 Test St", "Melbourne", "VIC", "3000", "AU");
        return new Listing(
            Guid.NewGuid(),
            "Test Listing",
            "Test Description",
            500000m,
            ListingType.ForSale,
            PropertyType.Apartment,
            address);
    }
}
