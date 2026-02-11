using Domain.Brokers;
using Domain.Buildings;
using Domain.Clients;
using Domain.Currencies;

namespace Application.Services.Policies;

public sealed record PolicyDraftPrerequisites(
    Client Client,
    Building Building,
    Broker Broker,
    Currency Currency,
    int CountryId,
    int CountyId,
    int CityId
);