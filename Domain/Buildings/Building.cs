using Domain.Common;
using Domain.Geography;
using Domain.Shared;
namespace Domain.Buildings;

public enum BuildingType
{
    Residential, Office, Industrial
}

public class Building
{
    public Guid Id { get; }
    public Guid ClientId { get; }
    public Address Address { get; }
    public City City { get; }
    public int ConstructionYear { get; private set; }
    public BuildingType BuildingType { get; }
    public int SurfaceArea { get; private set; }
    public Money InsuredValue { get; private set;  }
    public RiskProfile RiskProfile { get; private set; }

    private Building(
        Guid id,
        Guid clientId,
        Address address,
        City city,
        int constructionYear,
        BuildingType type,
        int surfaceArea,
        Money insuredValue,
        RiskProfile riskProfile)
    {
        Id = id;
        ClientId = clientId;
        Address = address;
        City = city;
        ConstructionYear = constructionYear;
        BuildingType = type;
        SurfaceArea = surfaceArea;
        InsuredValue = insuredValue;
        RiskProfile = riskProfile;
    }

    public static Result<Building> Create(
        Guid clientId,
        Address address,
        City city,
        int constructionYear,
        BuildingType type,
        int surfaceArea,
        Money insuredValue,
        RiskProfile riskProfile)
    {
        if (surfaceArea <= 0)
            return Result<Building>.Fail(ErrorType.Validation, "Surface area must be positive");

        if (constructionYear < 1700 || constructionYear > DateTime.UtcNow.Year)
            return Result<Building>.Fail(ErrorType.Validation, "Invalid construction year");

        return Result<Building>.Ok(
            new Building(
                Guid.NewGuid(),
                clientId,
                address,
                city,
                constructionYear,
                type,
                surfaceArea,
                insuredValue,
                riskProfile));
    }

    public Result UpdateConstructionYear(int year)
    {
        if (year < 1700 || year > DateTime.UtcNow.Year)
            return Result.Fail(ErrorType.Validation, "Invalid construction year");

        ConstructionYear = year;
        return Result.Ok();
    }

    public Result UpdateSurfaceArea(int surfaceArea)
    {
        if (surfaceArea <= 0)
            return Result.Fail(ErrorType.Validation, "Surface area must be positive");

        SurfaceArea = surfaceArea;
        return Result.Ok();
    }

    public Result UpdateInsuredValue(Money insuredValue)
    {
        InsuredValue = insuredValue;
        return Result.Ok();
    }

    public Result UpdateRiskProfile(RiskProfile riskProfile)
    {
        RiskProfile = riskProfile;
        return Result.Ok();
    }

}