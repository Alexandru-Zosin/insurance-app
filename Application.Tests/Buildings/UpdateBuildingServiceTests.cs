using Application.Services.Buildings.DTO;
using Application.Services.Buildings.UpdateBuilding;
using Domain.Buildings;
using Domain.Common;
using Domain.Geography;
using Domain.Shared;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;
namespace Application.Tests.Buildings;

public sealed class UpdateBuildingServiceTests
{
    private readonly Mock<IBuildingRepository> _buildings;
    private readonly UpdateBuildingService _service;
    private readonly Building _defaultBuilding;

    public UpdateBuildingServiceTests()
    {
        _buildings = new Mock<IBuildingRepository>(MockBehavior.Strict);
        _service = new UpdateBuildingService(_buildings.Object);
        _defaultBuilding = CreateDefaultBuilding();

        _buildings
            .Setup(r => r.GetByIdAsync(_defaultBuilding.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_defaultBuilding);
    }

    private static UpdateBuildingRequest CreateValidUpdateRequest(Guid buildingId)
    {
        return new UpdateBuildingRequest(
            BuildingId: buildingId,
            ConstructionYear: 2005,
            SurfaceArea: 150,
            InsuredValue: 120000m,
            Currency: "EUR",
            FloodRisk: true,
            EarthquakeRisk: false);
    }

    private static Building CreateDefaultBuilding()
    {
        var city = new City(1, "Cluj");
        var address = Address.Create("Main Street", "10").Value!;
        var money = Money.Create(100000m, "EUR").Value!;
        var risk = new RiskProfile(false, false);

        return Building.Create(
            Guid.NewGuid(),
            address,
            city,
            2000,
            BuildingType.Residential,
            120,
            money,
            risk)
            .Value!;
    }

    [Fact]
    public async Task HandleAsync_should_return_NotFound_when_building_does_not_exist()
    {
        var buildingId = Guid.NewGuid();

        _buildings
            .Setup(r => r.GetByIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Building?)null);

        var result = await _service.HandleAsync(
            CreateValidUpdateRequest(buildingId));

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Be("Building not found");

        _buildings.Verify(
            r => r.UpdateAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_construction_year_is_invalid()
    {
        var request = CreateValidUpdateRequest(_defaultBuilding.Id) with
        {
            ConstructionYear = 1400
        };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _buildings.Verify(
            r => r.UpdateAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_surface_area_is_invalid()
    {
        var request = CreateValidUpdateRequest(_defaultBuilding.Id) with
        {
            SurfaceArea = 0
        };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _buildings.Verify(
            r => r.UpdateAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_money_is_invalid()
    {
        var request = CreateValidUpdateRequest(_defaultBuilding.Id) with
        {
            InsuredValue = -1
        };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _buildings.Verify(
            r => r.UpdateAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_update_and_save_building_when_request_is_valid()
    {
        var request = CreateValidUpdateRequest(_defaultBuilding.Id);

        _buildings
            .Setup(r => r.UpdateAsync(_defaultBuilding, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.HandleAsync(request);
        // default building suffers mutations (updates through domain methods)

        result.IsSuccess.Should().BeTrue();
        result.Value!.Updated.Should().BeTrue();

        _defaultBuilding.ConstructionYear.Should().Be(request.ConstructionYear);
        _defaultBuilding.SurfaceArea.Should().Be(request.SurfaceArea);
        _defaultBuilding.InsuredValue.Amount.Should().Be(request.InsuredValue);
        _defaultBuilding.RiskProfile.FloodRisk.Should().Be(request.FloodRisk);

        _buildings.Verify(
            r => r.UpdateAsync(_defaultBuilding, It.IsAny<CancellationToken>()),
            Times.Once);
    }    
}
