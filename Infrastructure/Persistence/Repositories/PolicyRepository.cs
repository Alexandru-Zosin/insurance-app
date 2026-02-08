using Application.Common;
using Application.Repositories;
using Application.Services.Policies.DTOs;
using Domain.Policies;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfPolicy = Infrastructure.Persistence.Models.Policy;

namespace Infrastructure.Persistence.Repositories;

public sealed class PolicyRepository(InsuranceDbContext _db) : IPolicyRepository
{
    public void Add(Policy policy, CancellationToken cancellationToken)
    {
        if (policy is null) throw new ArgumentNullException(nameof(policy));

        var ef = ToEfModel(policy);
        _db.Policies.Add(ef);
    }

    public async Task UpdateAsync(Policy aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var ef = await _db.Policies
            .SingleOrDefaultAsync(x => x.PolicyNumber == aggregate.Number, ct)
            .ConfigureAwait(false);

        if (ef is null)
            throw new InvalidOperationException("Policy not found.");

        UpdateEfModel(ef, aggregate);
    }

    public async Task<Policy?> GetByIdAsync(Guid policyId, CancellationToken cancellationToken)
    {
        if (policyId == Guid.Empty) throw new ArgumentException("Policy id is required.", nameof(policyId));

        var ef = await _db.Policies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PolicyNumber == policyId, cancellationToken)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<Policy>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        if (clientId == Guid.Empty) throw new ArgumentException("Client id is required.", nameof(clientId));

        var rows = await _db.Policies
            .AsNoTracking()
            .Where(x => x.ClientKey == clientId)
            .OrderByDescending(x => x.CreationDate)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Policy>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken)
    {
        if (buildingId == Guid.Empty) throw new ArgumentException("Building id is required.", nameof(buildingId));

        var rows = await _db.Policies
            .AsNoTracking()
            .Where(x => x.BuildingKey == buildingId)
            .OrderByDescending(x => x.CreationDate)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Policy>> SearchAsync(PolicySearchCriteria criteria, PageRequest pageRequest, CancellationToken ct = default)
    {
        if (criteria is null) throw new ArgumentNullException(nameof(criteria));
        if (pageRequest is null) throw new ArgumentNullException(nameof(pageRequest));

        var q = _db.Policies.AsNoTracking().AsQueryable();

        if (criteria.ClientId is { } clientId && clientId != Guid.Empty)
            q = q.Where(x => x.ClientKey == clientId);

        if (criteria.BrokerId is { } brokerId && brokerId != Guid.Empty)
            q = q.Where(x => x.BrokerKey == brokerId);

        if (criteria.Status is { } status)
            q = q.Where(x => x.Status == status.ToString());

        if (criteria.StartDate is { } start)
            q = q.Where(x => x.StartDate >= start);

        if (criteria.EndDate is { } end)
            q = q.Where(x => x.EndDate <= end);

        var skip = (pageRequest.PageNumber - 1) * pageRequest.PageSize;

        var rows = await q
            .OrderByDescending(x => x.CreationDate)
            .ThenByDescending(x => x.PolicyId)
            .Skip(skip)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }


    private static EfPolicy ToEfModel(Policy domain)
    {
        return new EfPolicy
        {
            PolicyNumber = domain.Number,
            ClientKey = domain.ClientId,
            BuildingKey = domain.BuildingId,
            BrokerKey = domain.BrokerId,
            Status = domain.Status.ToString(),
            StartDate = domain.Tenure.StartDate,
            EndDate = domain.Tenure.EndDate,
            BasePremiumAmount = domain.BasePremium.Amount,
            FinalPremiumAmount = domain.FinalPremium.Amount,
            CurrencyCode = domain.CurrencyCode,
            CreationDate = domain.CreationDate,
            LastUpdateDate = domain.LastUpdateDate,
            CancellationReason = domain.CancellationReason,
            CancellationEffectiveDate = domain.CancellationEffectiveDate
        };
    }

    private static void UpdateEfModel(EfPolicy ef, Policy domain)
    {
        ef.ClientKey = domain.ClientId;
        ef.BuildingKey = domain.BuildingId;
        ef.BrokerKey = domain.BrokerId;
        ef.Status = domain.Status.ToString();
        ef.StartDate = domain.Tenure.StartDate;
        ef.EndDate = domain.Tenure.EndDate;
        ef.BasePremiumAmount = domain.BasePremium.Amount;
        ef.FinalPremiumAmount = domain.FinalPremium.Amount;
        ef.CurrencyCode = domain.CurrencyCode;
        ef.LastUpdateDate = domain.LastUpdateDate;
        ef.CancellationReason = domain.CancellationReason;
        ef.CancellationEffectiveDate = domain.CancellationEffectiveDate;
    }

    private static Policy ToDomain(EfPolicy ef)
    {
        return Policy.Rehydrate(
            number: ef.PolicyNumber,
            clientId: ef.ClientKey,
            buildingId: ef.BuildingKey,
            brokerId: ef.BrokerKey,
            tenure: ValidityPeriod.Create(ef.StartDate, ef.EndDate),
            basePremium: Money.Create(ef.BasePremiumAmount, ef.CurrencyCode),
            currencyCode: ef.CurrencyCode,
            finalPremium: Money.Create(ef.FinalPremiumAmount, ef.CurrencyCode),
            status: ParseStatus(ef.Status),
            creationDate: ef.CreationDate,
            lastUpdateDate: ef.LastUpdateDate,
            cancellationReason: ef.CancellationReason,
            cancellationEffectiveDate: ef.CancellationEffectiveDate);
    }

    private static PolicyStatus ParseStatus(string value)
    {
        if (Enum.TryParse<PolicyStatus>(value, ignoreCase: true, out var parsed))
            return parsed;

        throw new InvalidOperationException($"Unknown policy status '{value}'.");
    }
}
