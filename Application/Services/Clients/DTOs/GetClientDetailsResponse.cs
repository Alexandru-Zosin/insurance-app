using Domain.Buildings;
using Domain.Clients;
using Domain.Policies;

namespace Application.Services.Clients.DTOs;

public sealed record GetClientDetailsResponse(
    ClientDto Client,
    IReadOnlyList<BuildingDto> Buildings,
    IReadOnlyList<PolicyDto> Policies);

public sealed record ClientDto(
    Guid Id,
    string Type,
    string Name,
    string RegistrationNumber,
    string Email,
    string Phone,
    string? Street,
    string? Number)
{
    public static ClientDto From(Client client)
    {
        return new ClientDto(
            client.Id,
            client.Type.ToString(),
            client.Name,
            client.Identifier.Value,
            client.ContactInfo.Email,
            client.ContactInfo.Phone,
            client.Address.Street,
            client.Address.Number);
    }
}
public sealed record BuildingDto(
    Guid Id,
    int ConstructionYear,
    int SurfaceArea,
    decimal InsuredValue,
    string Currency,
    bool FloodRisk,
    bool EarthquakeRisk)
{
    public static BuildingDto From(Building building)
    {
        return new BuildingDto(
            building.Id,
            building.ConstructionYear,
            building.SurfaceArea,
            building.InsuredValue.Amount,
            building.InsuredValue.Currency,
            building.RiskProfile.FloodRisk,
            building.RiskProfile.EarthquakeRisk);
    }
}
public sealed record PolicyDto(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Premium,
    string Currency)
{
    public static PolicyDto From(Policy policy)
    {
        return new PolicyDto(
            policy.Id,
            policy.StartDate,
            policy.EndDate,
            policy.Premium.Amount,
            policy.Premium.Currency);
    }
}