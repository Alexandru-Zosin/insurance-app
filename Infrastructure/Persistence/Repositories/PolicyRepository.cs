using Application.Common;
using Application.Repositories;
using Application.Services.Policies.DTOs;
using Domain.Policies;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfPolicy = Infrastructure.Persistence.Models.Policy;

namespace Infrastructure.Persistence.Repositories;

public sealed class PolicyRepository(InsuranceDbContext dbContext) : IPolicyRepository
{
    public void Add(Policy policyToAdd, CancellationToken ct)
    {
        if (policyToAdd is null) throw new ArgumentNullException(nameof(policyToAdd));

        var policyRow = MapToEf(policyToAdd);
        dbContext.Policies.Add(policyRow);
    }

    public async Task UpdateAsync(Policy updatedPolicy, CancellationToken ct = default)
    {
        if (updatedPolicy is null) throw new ArgumentNullException(nameof(updatedPolicy));

        var existingPolicyRow = await dbContext.Policies
            .SingleOrDefaultAsync(x => x.PolicyNumber == updatedPolicy.Number, ct);

        if (existingPolicyRow is null)
            throw new InvalidOperationException("Policy not found.");

        MapOntoEf(existingPolicyRow, updatedPolicy);
    }

    public async Task<Policy?> GetByIdAsync(Guid policyNumber, CancellationToken ct)
    {
        if (policyNumber == Guid.Empty) 
            throw new ArgumentException("Policy id is required.", nameof(policyNumber));

        var policyRow = await dbContext.Policies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PolicyNumber == policyNumber, ct);

        return policyRow is null ? null : MapToDomain(policyRow);
    }

    public async Task<IReadOnlyList<Policy>> GetByClientIdAsync(Guid clientId, CancellationToken ct)
    {
        if (clientId == Guid.Empty) throw new ArgumentException("Client id is required.", nameof(clientId));

        var policyRows = await dbContext.Policies
            .AsNoTracking()
            .Where(x => x.ClientKey == clientId)
            .OrderByDescending(x => x.CreationDate)
            .ToListAsync(ct);

        return policyRows.Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<Policy>> GetByBuildingIdAsync(Guid buildingId, CancellationToken ct)
    {
        if (buildingId == Guid.Empty) throw new ArgumentException("Building id is required.", nameof(buildingId));

        var rows = await dbContext.Policies
            .AsNoTracking()
            .Where(x => x.BuildingKey == buildingId)
            .OrderByDescending(x => x.CreationDate)
            .ToListAsync(ct);

        return rows.Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<Policy>> SearchAsync(PolicySearchCriteria filter, PageRequest page, CancellationToken ct = default)
    {
        if (filter is null) throw new ArgumentNullException(nameof(filter));
        if (page is null) throw new ArgumentNullException(nameof(page));

        var query = dbContext.Policies.AsNoTracking().AsQueryable();

        if (filter.ClientId is Guid clientId && clientId != Guid.Empty)
            query = query.Where(x => x.ClientKey == clientId);

        if (filter.BrokerId is Guid brokerId && brokerId != Guid.Empty)
            query = query.Where(x => x.BrokerKey == brokerId);

        if (filter.Status is PolicyStatus status)
            query = query.Where(x => x.Status == status.ToString());

        if (filter.StartDate is DateOnly startDate)
            query = query.Where(x => x.StartDate >= startDate);

        if (filter.EndDate is DateOnly endDate)
            query = query.Where(x => x.EndDate <= endDate);

        var policyRows = await query
            .OrderByDescending(x => x.CreationDate)
            .ThenByDescending(x => x.PolicyId)
            .Skip(page.Offset)
            .Take(page.Take)
            .ToListAsync(ct);

        return policyRows.Select(MapToDomain).ToList();
    }


    private static EfPolicy MapToEf(Policy policy)
    {
        return new EfPolicy
        {
            PolicyNumber = policy.Number,
            ClientKey = policy.ClientId,
            BuildingKey = policy.BuildingId,
            BrokerKey = policy.BrokerId,
            Status = policy.Status.ToString(),
            StartDate = policy.Tenure.StartDate,
            EndDate = policy.Tenure.EndDate,
            BasePremiumAmount = policy.BasePremium.Amount,
            FinalPremiumAmount = policy.FinalPremium.Amount,
            CurrencyCode = policy.CurrencyCode,
            CreationDate = policy.CreationDate,
            LastUpdateDate = policy.LastUpdateDate,
            CancellationReason = policy.CancellationReason,
            CancellationEffectiveDate = policy.CancellationEffectiveDate
        };
    }

    private static void MapOntoEf(EfPolicy policyRow, Policy policy)
    {
        policyRow.ClientKey = policy.ClientId;
        policyRow.BuildingKey = policy.BuildingId;
        policyRow.BrokerKey = policy.BrokerId;
        policyRow.Status = policy.Status.ToString();
        policyRow.StartDate = policy.Tenure.StartDate;
        policyRow.EndDate = policy.Tenure.EndDate;
        policyRow.BasePremiumAmount = policy.BasePremium.Amount;
        policyRow.FinalPremiumAmount = policy.FinalPremium.Amount;
        policyRow.CurrencyCode = policy.CurrencyCode;
        policyRow.LastUpdateDate = policy.LastUpdateDate;
        policyRow.CancellationReason = policy.CancellationReason;
        policyRow.CancellationEffectiveDate = policy.CancellationEffectiveDate;
    }

    private static Policy MapToDomain(EfPolicy policyRow)
    {
        return Policy.Rehydrate(
            number: policyRow.PolicyNumber,
            clientId: policyRow.ClientKey,
            buildingId: policyRow.BuildingKey,
            brokerId: policyRow.BrokerKey,
            tenure: ValidityPeriod.Create(policyRow.StartDate, policyRow.EndDate),
            basePremium: Money.Create(policyRow.BasePremiumAmount, policyRow.CurrencyCode),
            currencyCode: policyRow.CurrencyCode,
            finalPremium: Money.Create(policyRow.FinalPremiumAmount, policyRow.CurrencyCode),
            status: ParseStatus(policyRow.Status),
            creationDate: policyRow.CreationDate,
            lastUpdateDate: policyRow.LastUpdateDate,
            cancellationReason: policyRow.CancellationReason,
            cancellationEffectiveDate: policyRow.CancellationEffectiveDate);
    }

    private static PolicyStatus ParseStatus(string statusValue)
    {
        if (Enum.TryParse<PolicyStatus>(statusValue, ignoreCase: true, out var policyStatus))
            return policyStatus;

        throw new InvalidOperationException($"Unknown policy status '{statusValue}'.");
    }
}