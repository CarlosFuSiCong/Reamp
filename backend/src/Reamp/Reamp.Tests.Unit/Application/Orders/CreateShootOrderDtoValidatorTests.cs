using FluentAssertions;
using FluentValidation.TestHelper;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.Orders.Validators;

namespace Reamp.Tests.Unit.Application.Orders;

public class PlaceOrderDtoValidatorTests
{
    private readonly PlaceOrderDtoValidator _validator;

    public PlaceOrderDtoValidatorTests()
    {
        _validator = new PlaceOrderDtoValidator();
    }

    [Fact]
    public void Validate_WithValidDto_ShouldPass()
    {
        var dto = new PlaceOrderDto
        {
            AgencyId = Guid.NewGuid(),
            StudioId = Guid.NewGuid(),
            ListingId = Guid.NewGuid(),
            Currency = "AUD"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyListingId_ShouldFail()
    {
        var dto = new PlaceOrderDto
        {
            AgencyId = Guid.NewGuid(),
            StudioId = Guid.NewGuid(),
            ListingId = Guid.Empty,
            Currency = "AUD"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ListingId);
    }

    [Fact]
    public void Validate_WithEmptyAgencyId_ShouldFail()
    {
        var dto = new PlaceOrderDto
        {
            AgencyId = Guid.Empty,
            StudioId = Guid.NewGuid(),
            ListingId = Guid.NewGuid(),
            Currency = "AUD"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AgencyId);
    }
}

