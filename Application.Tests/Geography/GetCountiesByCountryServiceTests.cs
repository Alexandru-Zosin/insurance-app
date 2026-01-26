using Application.Services.Geography.DTO;
using Application.UseCases.Geography;
using Domain.Common;
using Domain.Geography;
using FluentAssertions;
using Moq;

namespace Application.Tests.Geography;

public sealed class GetCountiesByCountryServiceTests
{
    private readonly Mock<ICountryRepository> _countries;
    private readonly Mock<ICountyRepository> _counties;
    private readonly GetCountiesByCountryService _service;

    public GetCountiesByCountryServiceTests()
    {
        _countries = new Mock<ICountryRepository>(MockBehavior.Strict);
        _counties = new Mock<ICountyRepository>(MockBehavior.Strict);

        _service = new GetCountiesByCountryService(
            _countries.Object,
            _counties.Object);
    }

    [Fact]
    public async Task HandleAsync_should_return_NotFound_when_country_does_not_exist()
    {
        var countryId = 1;

        _countries
            .Setup(r => r.GetByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?)null);

        var result = await _service.HandleAsync(
            new GetCountiesByCountryRequest(countryId));

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.ErrorMessage.Should().Be("Country not found");

        _counties.Verify(
            r => r.GetByCountryIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_should_return_mapped_counties_when_country_exists()
    {
        var countryId = 1;
        var country = new Country(countryId, "Romania");

        var county1 = new County(1, "Cluj");
        var county2 = new County(2, "Alba");

        _countries
            .Setup(r => r.GetByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(country);

        _counties
            .Setup(r => r.GetByCountryIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { county1, county2 });

        var result = await _service.HandleAsync(
            new GetCountiesByCountryRequest(countryId));

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Counties.Should().HaveCount(2);

        response.Counties[0].Id.Should().Be(county1.Id);
        response.Counties[0].Name.Should().Be(county1.Name);

        response.Counties[1].Id.Should().Be(county2.Id);
        response.Counties[1].Name.Should().Be(county2.Name);
    }
}
