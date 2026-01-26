using Application.Services.Buildings.DTO;
using Application.Services.Buildings.GetBuildingsForClient;
using Domain.Buildings;
using Domain.Geography;
using Domain.Shared;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Tests.Buildings;

public sealed class GetBuildingsForClientServiceTests
{
    private readonly Mock<IBuildingRepository> _buildings;
    private readonly GetBuildingsForClientService _service;

    public GetBuildingsForClientServiceTests()
    {
        _buildings = new Mock<IBuildingRepository>(MockBehavior.Strict);
        _service = new GetBuildingsForClientService(_buildings.Object);
    }

    [Fact]
    public async Task HandleAsync_should_return_empty_list_when_client_has_no_buildings()
    {
        var clientId = Guid.NewGuid();

        _buildings
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Building>());

        var result = await _service.HandleAsync(
            new GetBuildingsForClientRequest(clientId));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Buildings.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_should_return_mapped_buildings_for_client()
    {
        var clientId = Guid.NewGuid();
        var building = CreateBuilding(clientId);

        _buildings
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { building });

        var result = await _service.HandleAsync(
            new GetBuildingsForClientRequest(clientId));

        result.IsSuccess.Should().BeTrue();

        var retrievedBuilding = result.Value!.Buildings.Single();
        retrievedBuilding.Id.Should().Be(building.Id);
        retrievedBuilding.CityId.Should().Be(building.City.Id);
        retrievedBuilding.CityName.Should().Be(building.City.Name);
        retrievedBuilding.BuildingType.Should().Be(building.BuildingType.ToString());
        retrievedBuilding.SurfaceArea.Should().Be(building.SurfaceArea);
        retrievedBuilding.InsuredValue.Should().Be(building.InsuredValue.Amount);
        retrievedBuilding.Currency.Should().Be(building.InsuredValue.Currency);
    }

    private static Building CreateBuilding(Guid clientId)
    {
        var city = new City(1, "Cluj");
        var address = Address.Create("Main Street", "10").Value!;
        var money = Money.Create(100000m, "EUR").Value!;
        var risk = new RiskProfile(true, false);

        return Building.Create(
            clientId,
            address,
            city,
            2000,
            BuildingType.Residential,
            120,
            money,
            risk)
            .Value!;
    }
}
