using Application.Common;
using Application.Repositories;
using Application.Services.Metadata.DTOs;
using Application.Services.Shared.DTOs.MetadataDTOs;
using Domain.Configurations;

namespace Application.Services.Risks;

public sealed class RiskConfigurationService(
    IRiskConfigurationRepository riskConfigurationRepository,
    IUnitOfWork uow) : IRiskConfigurationService
{
    public async Task<Result<CreateRiskConfigurationResponse>> CreateBuildingTypeRiskConfigurationAsync(CreateBuildingTypeRiskRequest request, CancellationToken ct = default)
    {
        var newRiskConfiguration = BuildingTypeRiskConfiguration.Create(
            request.Name,
            request.Percentage, 
            request.IsActive,
            request.BuildingType);

        riskConfigurationRepository.Add(newRiskConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        return Result<CreateRiskConfigurationResponse>.Ok(new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(newRiskConfiguration)));
    }
    public async Task<Result<CreateRiskConfigurationResponse>> CreateCountryRiskConfigurationAsync(CreateCountryRiskRequest request, CancellationToken ct = default)
    {
        var newRiskConfiguration = CountryRiskConfiguration.Create(
            request.Name,
            request.Percentage,
            request.IsActive,
            request.CountryId);

        riskConfigurationRepository.Add(newRiskConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(newRiskConfiguration));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateCountyRiskConfigurationAsync(CreateCountyRiskRequest request, CancellationToken ct = default)
    {
        var newRiskConfiguration = CountyRiskConfiguration.Create(
            request.Name,
            request.Percentage,
            request.IsActive,
            request.CountyId);

        riskConfigurationRepository.Add(newRiskConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(newRiskConfiguration));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateCityRiskConfigurationAsync(CreateCityRiskRequest request, CancellationToken ct = default)
    {
        var newRiskConfiguration = CityRiskConfiguration.Create(
            request.Name,
            request.Percentage, 
            request.IsActive, 
            request.CityId);

        riskConfigurationRepository.Add(newRiskConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(newRiskConfiguration));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }
    
    public async Task<Result<CreateRiskConfigurationResponse>> CreateZoneCategoryRiskConfigurationAsync(CreateZoneCategoryRiskRequest request, CancellationToken ct = default)
    {
        var newRiskConfiguration = ZoneRiskConfiguration.Create(
            request.Name, 
            request.Percentage, 
            request.IsActive, 
            request.Category);

        riskConfigurationRepository.Add(newRiskConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(newRiskConfiguration));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<UpdateRiskConfigurationResponse>> UpdateRiskConfigurationAsync(Guid riskConfigurationId, UpdateRiskConfigurationRequest request, CancellationToken ct = default)
    {
        var riskConfiguration = await riskConfigurationRepository.GetByIdAsync(riskConfigurationId, ct);
        if (riskConfiguration is null)
            return Result<UpdateRiskConfigurationResponse>.Fail(ErrorType.NotFound, "Risk factor not found");

        riskConfiguration.Core
                         .UpdateName(request.Core.Name)
                         .UpdatePercentage(request.Core.Percentage);
        
        if (request.Core.IsActive) 
            riskConfiguration.Core.Activate(); 
        else 
            riskConfiguration.Core.Deactivate();

        await riskConfigurationRepository.UpdateAsync(riskConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new UpdateRiskConfigurationResponse(RiskConfigurationCoreDto.From(riskConfiguration));
        return Result<UpdateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<ListRiskConfigurationsResponse>> ListRiskConfigurationsAsync(CancellationToken ct = default)
    {
        var riskConfigurations = await riskConfigurationRepository.ListAsync(ct);

        var response = new ListRiskConfigurationsResponse(
            riskConfigurations.Select(RiskConfigurationListItemDto.From).ToArray());
        return Result<ListRiskConfigurationsResponse>.Ok(response);
    }
}
