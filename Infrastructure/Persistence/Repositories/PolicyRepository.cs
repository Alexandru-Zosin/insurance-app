using Application.Common;
using Application.Repositories;
using Application.Repositories.SearchCriteria;
using Domain.Policies;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfPolicy = Infrastructure.Persistence.Models.Policy;

namespace Infrastructure.Persistence.Repositories;

public sealed class PolicyRepository(InsuranceDbContext dbContext) : IPolicyRepository
{
    public void Add(Policy policyToAdd)
    {
        if (policyToAdd is null) 
            throw new ArgumentNullException(nameof(policyToAdd));

        var policyRow = MapToEf(policyToAdd);
        dbContext.Policies.Add(policyRow);
    }

    public async Task UpdateAsync(Policy updatedPolicy, CancellationToken ct = default)
    {
        if (updatedPolicy is null) 
            throw new ArgumentNullException(nameof(updatedPolicy));

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

    public async Task<IReadOnlyList<Policy>> ListAsync(
        PolicySearchCriteria filter,
        PageRequest? page = null,
        CancellationToken ct = default)
    {
        if (filter is null)
            throw new ArgumentNullException(nameof(filter));

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

        query = query
            .OrderByDescending(x => x.CreationDate)
            .ThenByDescending(x => x.PolicyId);

        if (page is not null)
            query = query.Skip(page.Offset).Take(page.Take);

        var policyRows = await query.ToListAsync(ct);
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
        return Policy.FromState(
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