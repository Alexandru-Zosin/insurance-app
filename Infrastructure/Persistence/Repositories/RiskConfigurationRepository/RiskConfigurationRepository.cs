using Application.Repositories;
using Domain.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository.MappersRegistry;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository;

public sealed class RiskConfigurationRepository(InsuranceDbContext _dbContext,
    IRiskConfigurationMapperRegistry _riskMapperRegistry) : IRiskConfigurationRepository
{
    public void Add(IRiskConfiguration riskConfigurationToAdd, CancellationToken ct = default)
    {
        if (riskConfigurationToAdd is null) throw new ArgumentNullException(nameof(riskConfigurationToAdd));

        var resolvedRiskMapper = _riskMapperRegistry.ResolveMapperForRiskConfiguration(riskConfigurationToAdd);
        var premiumRuleRow = resolvedRiskMapper.MapToEf(riskConfigurationToAdd);

        _dbContext.PremiumRules.Add(premiumRuleRow);
    }

    public async Task UpdateAsync(IRiskConfiguration updatedRiskConfiguration, CancellationToken ct = default)
    {
        if (updatedRiskConfiguration is null)
            throw new ArgumentNullException(nameof(updatedRiskConfiguration));

        var existingPremiumRuleRow = await _dbContext.PremiumRules
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == updatedRiskConfiguration.Core.Id, ct);

        if (existingPremiumRuleRow is null)
            throw new InvalidOperationException("Risk factor not found.");

        var resolvedRiskMapper = _riskMapperRegistry.ResolveMapperForRiskConfiguration(updatedRiskConfiguration);

        resolvedRiskMapper.MapOntoEf(existingPremiumRuleRow, updatedRiskConfiguration);
    }

    public async Task<IRiskConfiguration?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var premiumRuleRow = await _dbContext.PremiumRules
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == id, ct);

        if (premiumRuleRow is null)
            return null;

        if (!_riskMapperRegistry.TryResolveMapperForPremiumRuleRow(premiumRuleRow, out var resolvedRiskMapper))
            return null;

        return resolvedRiskMapper.MapToDomain(premiumRuleRow);
    }

    public async Task<IReadOnlyList<IRiskConfiguration>> ListAsync(CancellationToken ct = default)
    {
        var premiumRuleRows = await _dbContext.PremiumRules
            .AsNoTracking()
            .ToListAsync(ct);

        var riskConfigurations = new List<IRiskConfiguration>();

        foreach (var premiumRuleRow in premiumRuleRows)
        {
            if (!_riskMapperRegistry.TryResolveMapperForPremiumRuleRow(premiumRuleRow, out var resolvedRiskMapper))
                continue;

            riskConfigurations.Add(resolvedRiskMapper.MapToDomain(premiumRuleRow));
        }

        return riskConfigurations;
    }
}
