using Application.Services.Shared.DTOs.BrokerDTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Domain.Policies;
namespace Application.Services.Shared.DTOs.PolicyDTOs;

public sealed record PolicyDetailedDto(
    Guid Id,
    PolicyCoreDto Core,
    PolicyStatus Status,
    MoneyDto FinalPremium,
    DateOnly CreationDate,
    DateOnly? LastUpdateDate,
    string? CancellationReason,
    DateOnly? CancellationEffectiveDate,
    ClientListItemDto Client,
    BuildingListItemDto Building,
    BrokerListItemDto Broker)
{
    public static PolicyDetailedDto From(
        Policy e,
        ClientListItemDto client,
        BuildingListItemDto building,
        BrokerListItemDto broker) =>
        new(
            e.Number,
            PolicyCoreDto.From(e),
            e.Status,
            MoneyDto.From(e.FinalPremium),
            e.CreationDate,
            e.LastUpdateDate,
            e.CancellationReason,
            e.CancellationEffectiveDate,
            client,
            building,
            broker);
}