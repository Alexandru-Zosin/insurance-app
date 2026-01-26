using Application.Services.Geography.DTO;
using Application.UseCases.Geography;
using Domain.Geography;
using FluentAssertions;
using Moq;

namespace Application.Tests.Geography;

public sealed class GetCountriesTests
{
    private readonly Mock<ICountryRepository> _countries;
    private readonly GetCountries _service;

    public GetCountriesTests()
    {
        _countries = new Mock<ICountryRepository>(MockBehavior.Strict);
        _service = new GetCountries(_countries.Object);
    }

    [Fact]
    public async Task HandleAsync_should_return_mapped_countries()
    {
        var country1 = new Country(1, "Romania");
        var country2 = new Country(2, "Germany");

        _countries
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { country1, country2 });

        var result = await _service.HandleAsync(
            new GetCountriesRequest());

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Countries.Should().HaveCount(2);

        response.Countries[0].Id.Should().Be(country1.Id);
        response.Countries[0].Name.Should().Be(country1.Name);

        response.Countries[1].Id.Should().Be(country2.Id);
        response.Countries[1].Name.Should().Be(country2.Name);
    }
}
