using Domain.Common;
using Domain.Shared;
namespace Domain.Buildings;

public class Building
{
    private HashSet<RiskTag> _riskTags = [];

    public Guid Id { get; }
    public Guid OwnerClientId { get; }
    public Address Address { get; private set; }
    public int CityId { get; private set; }
    public int ConstructionYear { get; }
    public BuildingType BuildingType { get; }
    public int SurfaceArea { get; private set; }
    public Money InsuredValue { get; private set;  }
    public IReadOnlyCollection<RiskTag> RiskTags => _riskTags;

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
        IEnumerable<RiskCategory>? riskCategories = null
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

        if (riskCategories != null)
            foreach (var c in riskCategories.Distinct())
                b._riskTags.Add(new RiskTag(c));

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
        IEnumerable<RiskTag>? riskTags = null)
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

        if (riskTags != null)
            foreach (var tag in riskTags)
                b.AddRisk(tag);

        return b;
    }

    public Building AddRisk(RiskTag tag)
    {
        _riskTags.Add(tag);
        return this;
    }

    public Building RemoveRisk(RiskTag tag)
    {
        _riskTags.Remove(tag);
        return this;
    }

    public Building ChangeAddress(Address address)
    {
        ValidateAddress(address);
        Address = address;
        return this;
    }

    public Building ChangeSurfaceArea(int surfaceArea)
    {
        ValidateSurfaceArea(surfaceArea);
        SurfaceArea = surfaceArea;
        return this;
    }

    public Building ChangeInsuredValue(Money insuredValue)
    {
        ValidateInsuredValue(insuredValue);
        InsuredValue = insuredValue;
        return this;
    }

    private static void ValidateAddress(Address address)
    {
        if (address is null)
            throw new DomainException("Invalid address.");
    }

    private static void ValidateCity(int cityId)
    {
        if (cityId <= 0)
            throw new DomainException("Invalid City Id.");
    }

    private static void ValidateSurfaceArea(int surfaceArea)
    {
        if (surfaceArea <= 0)
            throw new DomainException("Surface area must be positive.");
    }

    private void ValidateInsuredValue(Money insuredValue)
    {
        if (insuredValue is null)
            throw new DomainException("Invalid insured value.");
    }

    private static void ValidateConstructionYear(int year)
    {
        if (year < 1600 || year > DateTime.UtcNow.Year)
            throw new DomainException("Invalid construction year.");
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