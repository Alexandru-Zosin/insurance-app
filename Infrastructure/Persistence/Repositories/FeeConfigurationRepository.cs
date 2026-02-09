using Application.Repositories;
using Domain.Configurations;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class FeeConfigurationRepository(InsuranceDbContext _dbContext) : IFeeConfigurationRepository
{
    private const string FeeRuleKind = "Fee";

    public void Add(FeeConfiguration feeConfigurationToAdd, CancellationToken ct = default)
    {
        var feeRuleRow = MapToEf(feeConfigurationToAdd);
        _dbContext.Set<PremiumRule>().Add(feeRuleRow);
    }

    public async Task<FeeConfiguration?> GetByIdAsync(Guid feeConfigurationId, CancellationToken ct = default)
    {
        var feeRuleRow = await _dbContext.Set<PremiumRule>()
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == feeConfigurationId 
                                    && r.RuleKind == FeeRuleKind, ct);

        return feeRuleRow is null ? null : MapToDomain(feeRuleRow);
    }

    public async Task<IReadOnlyList<FeeConfiguration>> ListAsync(CancellationToken ct = default)
    {
        var feeRuleRows = await _dbContext.Set<PremiumRule>()
            .AsNoTracking()
            .Where(r => r.RuleKind == "Fee")
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

        return feeRuleRows.Select(MapToDomain).ToList();
    }

    public async Task UpdateAsync(FeeConfiguration updatedFeeConfiguration, CancellationToken ct = default)
    {
        var existingFeeRuleRow = await _dbContext.Set<PremiumRule>()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == updatedFeeConfiguration.Id 
            && r.RuleKind == FeeRuleKind, ct);

        if (existingFeeRuleRow is null)
            throw new InvalidOperationException($"FeeConfiguration '{updatedFeeConfiguration.Id}' was not found.");

        MapOntoEf(existingFeeRuleRow, updatedFeeConfiguration);
    }

    public async Task DeactivateAsync(Guid feeConfigurationId, CancellationToken ct = default)
    {
        var existingFeeRuleRow = await _dbContext.Set<PremiumRule>()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == feeConfigurationId && r.RuleKind == "Fee", ct);

        if (existingFeeRuleRow is null)
            return;

        existingFeeRuleRow.IsActive = false;
    }

    public static PremiumRule MapToEf(FeeConfiguration feeConfiguration)
    {
        if (feeConfiguration is null) throw new ArgumentNullException(nameof(feeConfiguration));

        return new PremiumRule
        {
            PremiumRuleKey = feeConfiguration.Id,
            RuleKind = FeeRuleKind,
            Name = feeConfiguration.Name,
            Percentage = feeConfiguration.Percentage,
            IsActive = feeConfiguration.IsActive,
            FeeType = feeConfiguration.Type.ToString(),
            EffectiveFrom = feeConfiguration.ValidityPeriod?.StartDate,
            EffectiveTo = feeConfiguration.ValidityPeriod?.EndDate,
        };
    }

    public static void MapOntoEf(PremiumRule feeRuleRow, FeeConfiguration feeConfiguration)
    {
        if (feeRuleRow is null) throw new ArgumentNullException(nameof(feeRuleRow));
        if (feeConfiguration is null) throw new ArgumentNullException(nameof(feeConfiguration));

        if (!string.Equals(feeRuleRow.RuleKind, FeeRuleKind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{FeeRuleKind}'.");

        if (feeRuleRow.PremiumRuleKey != feeConfiguration.Id)
            throw new InvalidOperationException("Row key does not match feeConfiguration id.");

        feeRuleRow.Name = feeConfiguration.Name;
        feeRuleRow.Percentage = feeConfiguration.Percentage;
        feeRuleRow.IsActive = feeConfiguration.IsActive;
        feeRuleRow.FeeType = feeConfiguration.Type.ToString();
        feeRuleRow.EffectiveFrom = feeConfiguration.ValidityPeriod?.StartDate;
        feeRuleRow.EffectiveTo = feeConfiguration.ValidityPeriod?.EndDate;
    }

    public static FeeConfiguration MapToDomain(PremiumRule feeRuleRow)
    {
        if (feeRuleRow is null) throw new ArgumentNullException(nameof(feeRuleRow));
        if (!string.Equals(feeRuleRow.RuleKind, FeeRuleKind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{FeeRuleKind}'.");

        if (string.IsNullOrWhiteSpace(feeRuleRow.FeeType))
            throw new InvalidOperationException("Fee rule row requires FeeType.");

        var feeType = Enum.Parse<FeeType>(feeRuleRow.FeeType, ignoreCase: true);

        var validityPeriod = ValidityPeriod.CreateOptional(feeRuleRow.EffectiveFrom, feeRuleRow.EffectiveTo);

        return FeeConfiguration.Rehydrate(
            id: feeRuleRow.PremiumRuleKey,
            name: feeRuleRow.Name,
            type: feeType,
            percentage: feeRuleRow.Percentage,
            validityPeriod: validityPeriod!,
            isActive: feeRuleRow.IsActive);
    }
}