using Application.Services.Buildings;
using Application.Services.Buildings.DTO;
using Domain.Buildings;
using Domain.Common;
using Domain.Geography;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

/*
 * City not found - NotFound
Invalid address - validation error
Invalid money - validation error
Invalid building creation (e.g. surface area) - validation error
Happy path - building is created and saved
 */

namespace Application.Tests.Buildings;

public sealed class RegisterBuildingServiceTests
{
    private readonly Mock<IBuildingRepository> _buildings;
    private readonly Mock<ICityRepository> _cities;
    private readonly RegisterBuildingService _service;
    private readonly City _defaultCity;

    public RegisterBuildingServiceTests()
    {
        _buildings = new Mock<IBuildingRepository>(MockBehavior.Strict);
        _cities = new Mock<ICityRepository>(MockBehavior.Strict);

        _defaultCity = new City(1, "Cluj");
        _cities
        .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(_defaultCity);

        _service = new RegisterBuildingService(
            _buildings.Object,
            _cities.Object);
    }

    private static RegisterBuildingRequest CreateValidRequest()
    {
        return new RegisterBuildingRequest(
            ClientId: Guid.NewGuid(),
            CityId: 1,
            Street: "Main Street",
            Number: "10",
            ConstructionYear: 2000,
            BuildingType: BuildingType.Residential.ToString(),
            SurfaceArea: 120,
            InsuredValue: 100000m,
            Currency: "EUR",
            FloodRisk: true,
            EarthquakeRisk: false);
    }

    [Fact]
    public async Task HandleAsync_should_return_NotFound_when_city_does_not_exist()
    {
        var request = CreateValidRequest();

        _cities
            .Setup(r => r.GetByIdAsync(request.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City?)null); // Override for when city doesn't exist

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Be("City not found");

        _buildings.Verify(
            r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_address_is_invalid()
    {
        var request = CreateValidRequest() with { Street = "" };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _buildings.Verify(
            r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_money_is_invalid()
    {
        var request = CreateValidRequest() with { InsuredValue = -1 };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _buildings.Verify(
            r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_building_creation_fails()
    {
        var request = CreateValidRequest() with { SurfaceArea = 0 };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _buildings.Verify(
            r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_create_and_save_building_when_request_is_valid()
    {
        var request = CreateValidRequest();

        _buildings
            .Setup(r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.BuildingId.Should().NotBeEmpty();

        _buildings.Verify(
            r => r.AddAsync(It.Is<Building>(b =>
                b.ClientId == request.ClientId &&
                b.City.Id == request.CityId &&
                b.SurfaceArea == request.SurfaceArea),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
