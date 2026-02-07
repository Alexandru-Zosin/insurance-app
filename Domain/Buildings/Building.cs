using Domain.Common;
using Domain.Configurations;
using Domain.Shared;
namespace Domain.Buildings;

public class Building
{
    private HashSet<ZoneRiskCategory> _zoneRiskCategories = [];

    public Guid Id { get; }
    public Guid OwnerClientId { get; }
    public Address Address { get; private set; }
    public int CityId { get; private set; }
    public int ConstructionYear { get; }
    public BuildingType BuildingType { get; }
    public int SurfaceArea { get; private set; }
    public Money InsuredValue { get; private set;  }
    public IReadOnlyCollection<ZoneRiskCategory> ZoneRiskCategories => _zoneRiskCategories;

    private Building(
        Guid id,
        Guid ownerClientId,
        Address address,
        int cityId,
        int constructionYear,
        BuildingType type,
        int surfaceArea,
        Money insuredValue
        )
    {
        Id = id;
        OwnerClientId = ownerClientId;
        Address = address;
        CityId = cityId;
        ConstructionYear = constructionYear;
        BuildingType = type;
        SurfaceArea = surfaceArea;
        InsuredValue = insuredValue;

        ValidateInvariants();
    }

    public static Building RegisterForClient(
        Guid ownerClientId,
        Address address,
        int cityId,
        int constructionYear,
        BuildingType type,
        int surfaceArea,
        Money insuredValue,
        IEnumerable<ZoneRiskCategory> zoneRiskCategories
 )
    {
        var b = new Building(
                Guid.NewGuid(),
                ownerClientId,
                address,
                cityId,
                constructionYear,
                type,
                surfaceArea,
                insuredValue);

        foreach (var c in zoneRiskCategories.Distinct())
            b._zoneRiskCategories.Add(c);

        return b;
    }

    public static Building Rehydrate(
        Guid id,
        Guid ownerClientId,
        Address address,
        int cityId,
        int constructionYear,
        BuildingType type,
        int surfaceArea,
        Money insuredValue,
        IEnumerable<ZoneRiskCategory> zoneRiskCategories)
    {
        var b = new Building(
            id,
            ownerClientId,
            address,
            cityId,
            constructionYear,
            type,
            surfaceArea,
            insuredValue);

        foreach (var tag in zoneRiskCategories)
            b.AddRisk(tag);

        return b;
    }

    public Building AddRisk(ZoneRiskCategory tag)
    {
        _zoneRiskCategories.Add(tag);
        return this;
    }

    public Building RemoveRisk(ZoneRiskCategory tag)
    {
        _zoneRiskCategories.Remove(tag);
        return this;
    }

    public Building UpdateAddress(Address address)
    {
        ValidateAddress(address);
        Address = address;
        return this;
    }

    public Building UpdateSurfaceArea(int surfaceArea)
    {
        ValidateSurfaceArea(surfaceArea);
        SurfaceArea = surfaceArea;
        return this;
    }

    public Building UpdateInsuredValue(Money insuredValue)
    {
        ValidateInsuredValue(insuredValue);
        InsuredValue = insuredValue;
        return this;
    }

    private static void ValidateAddress(Address address)
    {
        if (address is null)
            throw new DomainException(BuildingConstants.InvalidAddressMsg);
    }

    private static void ValidateCity(int cityId)
    {
        if (cityId <= 0)
            throw new DomainException(BuildingConstants.InvalidCityIdMsg);
    }

    private static void ValidateSurfaceArea(int surfaceArea)
    {
        if (surfaceArea <= 0)
            throw new DomainException(BuildingConstants.SurfaceAreaMustBePositiveMsg);
    }

    private void ValidateInsuredValue(Money insuredValue)
    {
        if (insuredValue is null)
            throw new DomainException(BuildingConstants.InvalidInsuredValueMsg);
    }

    private static void ValidateConstructionYear(int year)
    {
        if (year < BuildingConstants.ConstructionYearMin || year > DateTime.UtcNow.Year)
            throw new DomainException(BuildingConstants.InvalidConstructionYearMsg);
    }

    private void ValidateInvariants()
    {
        ValidateAddress(Address);
        ValidateCity(CityId);
        ValidateSurfaceArea(SurfaceArea);
        ValidateInsuredValue(InsuredValue);
        ValidateConstructionYear(ConstructionYear);
    }
}