using Application.Services.Clients.DTO;
using Application.UseCases.Clients;
using Domain.Clients;
using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Tests.Clients;

public sealed class UpdateClientServiceTests
{
    private readonly Mock<IClientRepository> _clients;
    private readonly UpdateClientService _service;
    private readonly Client _defaultClient;

    public UpdateClientServiceTests()
    {
        _clients = new Mock<IClientRepository>(MockBehavior.Strict);
        _service = new UpdateClientService(_clients.Object);

        _defaultClient = CreateDefaultClient();

        _clients
            .Setup(r => r.GetByIdAsync(_defaultClient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_defaultClient);
    }

    private static UpdateClientRequest CreateValidUpdateRequest(Guid clientId)
    {
        return new UpdateClientRequest(
            ClientId: clientId,
            Name: "John Updated",
            Email: "john.updated@test.com",
            Phone: "12345",
            Street: "Main Street",
            Number: "10");
    }

    private static Client CreateDefaultClient()
    {
        var id = IdentificationNumber.Create("ID123").Value!;
        var contact = ContactInfo.Create("john@test.com", "123").Value!;
        var address = Address.Create("Old Street", "5").Value!;

        return Client.Create(
            ClientType.Individual,
            "John Doe",
            id,
            contact,
            address)
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
            CreateValidUpdateRequest(clientId));

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Be("Client not found");

        _clients.Verify(
            r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_name_is_invalid()
    {
        var request = CreateValidUpdateRequest(_defaultClient.Id) with
        {
            Name = ""
        };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _clients.Verify(
            r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_contact_info_is_invalid()
    {
        var request = CreateValidUpdateRequest(_defaultClient.Id) with
        {
            Email = "",
            Phone = ""
        };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _clients.Verify(
            r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_partial_address_is_invalid()
    {
        var request = CreateValidUpdateRequest(_defaultClient.Id) with
        {
            Street = "Main Street",
            Number = ""
        };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _clients.Verify(
            r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_update_and_save_client_when_request_is_valid()
    {
        var request = CreateValidUpdateRequest(_defaultClient.Id);

        _clients
            .Setup(r => r.UpdateAsync(_defaultClient, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Updated.Should().BeTrue();

        _defaultClient.Name.Should().Be(request.Name);
        _defaultClient.ContactInfo.Email.Should().Be(request.Email);
        _defaultClient.ContactInfo.Phone.Should().Be(request.Phone);
        _defaultClient.Address!.Street.Should().Be(request.Street);
        _defaultClient.Address.Number.Should().Be(request.Number);

        _clients.Verify(
            r => r.UpdateAsync(_defaultClient, It.IsAny<CancellationToken>()),
            Times.Once);
    }    
}
