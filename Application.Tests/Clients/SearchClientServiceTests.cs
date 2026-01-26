using Application.Services.Clients.DTO;
using Application.UseCases.Clients;
using Domain.Clients;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Tests.Clients;

public sealed class SearchClientsServiceTests
{
    private readonly Mock<IClientRepository> _clients;
    private readonly SearchClientsService _service;

    public SearchClientsServiceTests()
    {
        _clients = new Mock<IClientRepository>(MockBehavior.Strict);
        _service = new SearchClientsService(_clients.Object);
    }

    private static Client CreateClientWithAddress()
    {
        var id = IdentificationNumber.Create("ID123").Value!;
        var contact = ContactInfo.Create("john@test.com", "123").Value!;
        var address = Address.Create("Main Street", "10").Value!;

        return Client.Create(
            ClientType.Individual,
            "John Doe",
            id,
            contact,
            address)
            .Value!;
    }

    [Fact]
    public async Task HandleAsync_should_return_single_client_when_identifier_is_provided_and_found()
    {
        var client = CreateClientWithAddress();
        var identifier = client.Identifier.Value;

        _clients
            .Setup(r => r.GetByRegistrationNumberAsync(identifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var result = await _service.HandleAsync(
            new SearchClientsRequest(Name: null, Identifier: identifier));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Clients.Should().HaveCount(1);
        result.Value.Clients[0].Id.Should().Be(client.Id);
    }

    [Fact]
    public async Task HandleAsync_should_return_empty_list_when_identifier_is_provided_but_not_found()
    {
        var identifier = "ID_NOT_FOUND";

        _clients
            .Setup(r => r.GetByRegistrationNumberAsync(identifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var result = await _service.HandleAsync(
            new SearchClientsRequest(Name: null, Identifier: identifier));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Clients.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_should_search_by_name_when_identifier_is_not_provided()
    {
        var client = CreateClientWithAddress();

        _clients
            .Setup(r => r.SearchByNameAsync("John", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client });

        var result = await _service.HandleAsync(
            new SearchClientsRequest(Name: "John", Identifier: null));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Clients.Should().HaveCount(1);
        result.Value.Clients[0].Name.Should().Be(client.Name);
    }

    [Fact]
    public async Task HandleAsync_should_return_empty_list_when_no_search_criteria_is_provided()
    {
        var result = await _service.HandleAsync(
            new SearchClientsRequest(Name: null, Identifier: null));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Clients.Should().BeEmpty();

        _clients.Verify(
            r => r.GetByRegistrationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _clients.Verify(
            r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
