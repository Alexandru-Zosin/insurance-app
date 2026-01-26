using Application.Services.Geography.DTO;
using Application.UseCases.Geography;
using Domain.Geography;
using FluentAssertions;
using Moq;

namespace Application.Tests.Geography;

public sealed class GetCitiesByCountyServiceTests
{
    private readonly Mock<ICityRepository> _cities;
    private readonly GetCitiesByCountyService _service;

    public GetCitiesByCountyServiceTests()
    {
        _cities = new Mock<ICityRepository>(MockBehavior.Strict);
        _service = new GetCitiesByCountyService(_cities.Object);
    }

    [Fact]
    public async Task HandleAsync_should_return_mapped_cities_for_county()
    {
        var countyId = 1;

        var city1 = new City(1, "Cluj");
        var city2 = new City(2, "Turda");

        _cities
            .Setup(r => r.GetByCountyIdAsync(countyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { city1, city2 });

        var result = await _service.HandleAsync(
            new GetCitiesByCountyRequest(countyId));

        result.IsSuccess.Should().BeTrue();

        var response = result.Value!;
        response.Cities.Should().HaveCount(2);

        response.Cities[0].Id.Should().Be(city1.Id);
        response.Cities[0].Name.Should().Be(city1.Name);

        response.Cities[1].Id.Should().Be(city2.Id);
        response.Cities[1].Name.Should().Be(city2.Name);
    }
}
