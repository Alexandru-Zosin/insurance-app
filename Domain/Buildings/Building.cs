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
    public Address Address { get; private set; }
    public City City { get; private set; }
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

        ValidateInvariants();
    }

    public static Building Create(
        Guid clientId,
        Address address,
        City city,
        int constructionYear,
        BuildingType type,
        int surfaceArea,
        Money insuredValue,
        RiskProfile riskProfile)
    {
        return new Building(
                Guid.NewGuid(),
                clientId,
                address,
                city,
                constructionYear,
                type,
                surfaceArea,
                insuredValue,
                riskProfile);
    }

    public static Building Rehydrate(
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
        return new Building(
            id,
            clientId,
            address,
            city,
            constructionYear,
            type,
            surfaceArea,
            insuredValue,
            riskProfile);
    }

    public Building ChangeAddress(Address address)
    {
        Address = address;
        return this;
    }

    public Building ChangeCity(City city)
    {
        City = city;
        return this;
    }

    public Building UpdateConstructionYear(int year)
    {
        EnsureConstructionYear(year);
        ConstructionYear = year;
        return this;
    }

    private static void EnsureConstructionYear(int year)
    {
        var currentYear = DateTime.UtcNow.Year;
        if (year < 1600 || year > currentYear) // will switch to config file with defined values
            throw new DomainException("Invalid construction year.");
    }
    
    public Building UpdateSurfaceArea(int surfaceArea)
    {
        if (surfaceArea <= 0) 
            throw new DomainException("Surface area must be positive.");
        SurfaceArea = surfaceArea;
        return this;
    }

    public Building UpdateInsuredValue(Money insuredValue)
    {
        InsuredValue = insuredValue;
        return this;
    }

    public Building UpdateRiskProfile(RiskProfile riskProfile)
    {
        if (riskProfile is null) 
            throw new DomainException("Risk profile is required.");
        RiskProfile = riskProfile;
        return this;
    }

    private void ValidateInvariants()
    {
    }
}