using Application.Services.Clients.DTO;
using Application.UseCases.Clients;
using Domain.Buildings;
using Domain.Clients;
using Domain.Common;
using Domain.Geography;
using Domain.Policies;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Tests.Clients;

public sealed class GetClientDetailsServiceTests
{
    private readonly Mock<IClientRepository> _clients;
    private readonly Mock<IBuildingRepository> _buildings;
    private readonly Mock<IPolicyRepository> _policies;
    private readonly GetClientDetailsService _service;

    public GetClientDetailsServiceTests()
    {
        _clients = new Mock<IClientRepository>(MockBehavior.Strict);
        _buildings = new Mock<IBuildingRepository>(MockBehavior.Strict);
        _policies = new Mock<IPolicyRepository>(MockBehavior.Strict);

        _service = new GetClientDetailsService(
            _clients.Object,
            _buildings.Object,
            _policies.Object);
    }

    private static Client CreateClient()
    {
        var id = IdentificationNumber.Create("ID123").Value!;
        var contact = ContactInfo.Create("test@test.com", "123").Value!;
        var address = Address.Create("Null Street", "10").Value!;

        return Client.Create(
            ClientType.Individual,
            "John Doe",
            id,
            contact,
            address)
            .Value!;
    }

    private static Building CreateBuilding(Guid clientId)
    {
        var city = new City(1, "Cluj");
        var address = Address.Create("Main Street", "10").Value!;
        var money = Money.Create(100000m, "EUR").Value!;
        var risk = new RiskProfile(false, false);

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

    private static Policy CreatePolicy(Guid clientId, Guid buildingId)
    {
        return Policy.Issue(
            clientId,
            buildingId,
            Guid.NewGuid(),
            Money.Create(1000m, "EUR").Value!,
            new DateOnly(2024, 1, 1),
            new DateOnly(2025, 1, 1))
            .Value!;
    }

    [Fact]
    public async Task HandleAsync_should_return_NotFound_when_client_does_not_exist()
    {
        var clientId = Guid.NewGuid();

        _clients
            .Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var result = await _service.HandleAsync(
            new GetClientDetailsRequest(clientId));

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Be("Client not found");

        _buildings.Verify(
            r => r.GetByClientIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _policies.Verify(
            r => r.GetByClientIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_return_client_with_empty_buildings_and_policies()
    {
        var client = CreateClient();
        var clientId = client.Id;

        _clients
            .Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _buildings
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Building>());

        _policies
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Policy>());

        var result = await _service.HandleAsync(
            new GetClientDetailsRequest(clientId));

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Client.Id.Should().Be(client.Id);
        response.Client.Name.Should().Be(client.Name);
        response.Buildings.Should().BeEmpty();
        response.Policies.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_should_return_client_with_buildings_and_policies()
    {
        var client = CreateClient();
        var clientId = client.Id;

        var building = CreateBuilding(clientId);
        var policy = CreatePolicy(clientId, building.Id);

        _clients
            .Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _buildings
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { building });

        _policies
            .Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { policy });

        var result = await _service.HandleAsync(
            new GetClientDetailsRequest(clientId));

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Buildings.Should().HaveCount(1);
        response.Policies.Should().HaveCount(1);

        response.Buildings[0].Id.Should().Be(building.Id);
        response.Policies[0].Id.Should().Be(policy.Id);
    }
   
}
