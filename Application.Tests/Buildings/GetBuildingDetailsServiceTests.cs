using Application.Services.Buildings;
using Application.Services.Buildings.DTO;
using Domain.Buildings;
using Domain.Common;
using Domain.Geography;
using Domain.Policies;
using Domain.Shared;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Tests.Buildings;

/*
 * We need to tes building not found (verify NotFound & next repo (policies) is NOT called)
 * building found w/out entities (verify mapping & empty list)
 * building found w/ entities (verify mapping)
 */

public sealed class GetBuildingDetailsServiceTests
{
    private readonly Mock<IBuildingRepository> _buildings;
    private readonly Mock<IPolicyRepository> _policies;

    private readonly GetBuildingDetailsService _service;

    public GetBuildingDetailsServiceTests()
    {
        _buildings = new Mock<IBuildingRepository>(MockBehavior.Strict);
        _policies = new Mock<IPolicyRepository>(MockBehavior.Strict);

        _service = new GetBuildingDetailsService(_buildings.Object, _policies.Object);
    }

    private static Building CreateBuilding(Guid clientId)
    {
        var city = new City(1, "Cluj");
        var address = Address.Create("Main Street", "10").Value!;
        var money = Money.Create(100000, "EUR").Value!;
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

    [Fact]
    public async Task HandleAsync_should_return_NotFound_when_building_doesnt_exist()
    {
        var buildingId = Guid.NewGuid();

        _buildings
            .Setup(r => r.GetByIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Building?)null);

        var result = await _service.HandleAsync(new GetBuildingDetailsRequest(buildingId));

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Be("Building not found");

        _policies.Verify(
            r => r.GetByBuildingIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_return_building_with_empty_policies()
    {
        var clientId = Guid.NewGuid();
        var building = CreateBuilding(clientId);
        var buildingId = building.Id;

        _buildings
            .Setup(r => r.GetByIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);

        _policies
            .Setup(r => r.GetByBuildingIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Policy>());

        var result = await _service.HandleAsync(
            new GetBuildingDetailsRequest(buildingId));

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Building.Id.Should().Be(building.Id);
        response.Building.CityName.Should().Be("Cluj");
        response.Building.SurfaceArea.Should().Be(120);
        response.Building.FloodRisk.Should().BeTrue();
        response.Policies.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_should_return_building_with_policies()
    {
        var clientId = Guid.NewGuid();
        var building = CreateBuilding(clientId);
        var buildingId = building.Id;

        var policy = Policy.Issue(
            building.ClientId,
            building.Id,
            Guid.NewGuid(),
            Money.Create(1000, "EUR").Value!,
            new DateOnly(2024, 1, 1),
            new DateOnly(2025, 1, 1))
            .Value!;

        _buildings
            .Setup(r => r.GetByIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);

        _policies
            .Setup(r => r.GetByBuildingIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { policy });

        var result = await _service.HandleAsync(
            new GetBuildingDetailsRequest(buildingId));

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Policies.Should().HaveCount(1);

        var retrievedPolicy = response.Policies[0];
        retrievedPolicy.Premium.Should().Be(1000);
        retrievedPolicy.Currency.Should().Be("EUR");
    }
}
