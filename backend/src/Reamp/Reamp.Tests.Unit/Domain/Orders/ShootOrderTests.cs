using FluentAssertions;
using Reamp.Domain.Shoots.Entities;
using Reamp.Domain.Shoots.Enums;

namespace Reamp.Tests.Unit.Domain.Orders;

public class ShootOrderTests
{
    [Fact]
    public void Place_WithValidData_ShouldCreateOrder()
    {
        var agencyId = Guid.NewGuid();
        var studioId = Guid.NewGuid();
        var listingId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var currency = "AUD";

        var order = ShootOrder.Place(agencyId, studioId, listingId, createdBy, currency);

        order.Should().NotBeNull();
        order.ListingId.Should().Be(listingId);
        order.AgencyId.Should().Be(agencyId);
        order.StudioId.Should().Be(studioId);
        order.Status.Should().Be(ShootOrderStatus.Placed);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var order = CreateTestOrder();
        var reason = "Client requested cancellation";

        order.Cancel(reason);

        order.Status.Should().Be(ShootOrderStatus.Cancelled);
    }

    [Fact]
    public void AssignPhotographer_ShouldSetPhotographerId()
    {
        var order = CreateTestOrder();
        var photographerId = Guid.NewGuid();

        order.AssignPhotographer(photographerId);

        order.AssignedPhotographerId.Should().Be(photographerId);
    }

    [Fact]
    public void UnassignPhotographer_ShouldClearPhotographerId()
    {
        var order = CreateTestOrder();
        var photographerId = Guid.NewGuid();
        order.AssignPhotographer(photographerId);

        order.UnassignPhotographer();

        order.AssignedPhotographerId.Should().BeNull();
    }

    private static ShootOrder CreateTestOrder()
    {
        return ShootOrder.Place(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "AUD");
    }
}
