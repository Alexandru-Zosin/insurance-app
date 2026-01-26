using Application.Services.Clients.DTO;
using Application.UseCases.Clients;
using Domain.Clients;
using Domain.Common;
using Domain.Shared;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Tests.Clients;

public sealed class CreateClientServiceTests
{
    private readonly Mock<IClientRepository> _clients;
    private readonly CreateClientService _service;

    public CreateClientServiceTests()
    {
        _clients = new Mock<IClientRepository>(MockBehavior.Strict);

        // default behavior - no existing client
        _clients
            .Setup(r => r.GetByRegistrationNumberAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        _service = new CreateClientService(_clients.Object);
    }

    private static CreateClientRequest CreateValidRequest()
    {
        return new CreateClientRequest(
        ClientType.Individual.ToString(),
        "John Doe",
        "1234567890123",
        "john@doe.com",
        "+40123456789",
        "Main Street",
        "10");
    }

    private static Client CreateExistingClient()
    {
        var id = IdentificationNumber.Create("1234567890123").Value!;
        var contact = Domain.ValueObjects.ContactInfo.Create(
            "john@doe.com",
            "+40123456789").Value!;
        var address = Address.Create("Main Street", "10").Value!;

        return Client.Create(
            ClientType.Individual,
            "John Doe",
            id,
            contact,
            address).Value!;
    }

    [Fact]
    public async Task HandleAsync_should_return_Conflict_when_registration_number_exists()
    {
        var request = CreateValidRequest();

        Client _existingClient = CreateExistingClient();
        _clients
            .Setup(r => r.GetByRegistrationNumberAsync(
                request.RegistrationNumber,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingClient);

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.ErrorMessage.Should().Be("Identification number already exists");

        _clients.Verify(
            r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_identification_number_is_invalid()
    {
        var request = CreateValidRequest()
            with
        { RegistrationNumber = "" };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _clients.Verify(
            r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_contact_info_is_invalid()
    {
        var request = CreateValidRequest()
            with
        { Email = "not-an-email" };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _clients.Verify(
            r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_fail_when_address_is_invalid()
    {
        var request = CreateValidRequest()
            with
        { Number = "" };

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);

        _clients.Verify(
            r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_create_and_save_client_when_request_is_valid()
    {
        var request = CreateValidRequest();

        _clients
            .Setup(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.HandleAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ClientId.Should().NotBeEmpty();

        _clients.Verify(
            r => r.AddAsync(
                It.Is<Client>(c =>
                    c.Name == request.Name &&
                    c.Identifier.Value == request.RegistrationNumber),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
