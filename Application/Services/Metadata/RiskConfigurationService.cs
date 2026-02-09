using Application.Common;
using Application.Repositories;
using Application.Services.Metadata.DTOs;
using Application.Services.Shared.DTOs.MetadataDTOs;
using Domain.Configurations;

namespace Application.Services.Risks;

public sealed class RiskConfigurationService(
    IRiskConfigurationRepository _risks,
    IUnitOfWork _uow) : IRiskConfigurationService
{
    public async Task<Result<ListRiskConfigurationsResponse>> ListAsync(CancellationToken ct = default)
    {
        var list = await _risks.ListAsync(ct);

        var response = new ListRiskConfigurationsResponse(
            list.Select(RiskConfigurationListItemDto.From).ToArray());
        return Result<ListRiskConfigurationsResponse>.Ok(response);
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateCountryAsync(CreateCountryRiskRequest request, CancellationToken ct = default)
    {
        var risk = CountryRiskConfiguration.Create(request.Name, request.Percentage, request.IsActive, request.CountryId);

        _risks.Add(risk, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(risk));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateCountyAsync(CreateCountyRiskRequest request, CancellationToken ct = default)
    {
        var risk = CountyRiskConfiguration.Create(request.Name, request.Percentage, request.IsActive, request.CountyId);

        _risks.Add(risk, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(risk));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateCityAsync(CreateCityRiskRequest request, CancellationToken ct = default)
    {
        var risk = CityRiskConfiguration.Create(request.Name, request.Percentage, request.IsActive, request.CityId);

        _risks.Add(risk, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(risk));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateBuildingTypeAsync(CreateBuildingTypeRiskRequest request, CancellationToken ct = default)
    {
        var risk = BuildingTypeRiskConfiguration.Create(request.Name, request.Percentage, request.IsActive, request.BuildingType);

        _risks.Add(risk, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<CreateRiskConfigurationResponse>.Ok(new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(risk)));
    }

    public async Task<Result<CreateRiskConfigurationResponse>> CreateZoneCategoryAsync(CreateZoneCategoryRiskRequest request, CancellationToken ct = default)
    {
        var risk = ZoneRiskConfiguration.Create(request.Name, request.Percentage, request.IsActive, request.Category);

        _risks.Add(risk, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new CreateRiskConfigurationResponse(RiskConfigurationListItemDto.From(risk));
        return Result<CreateRiskConfigurationResponse>.Ok(response);
    }

    public async Task<Result<UpdateRiskConfigurationResponse>> UpdateCoreAsync(Guid id, UpdateRiskConfigurationRequest request, CancellationToken ct = default)
    {
        var existing = await _risks.GetByIdAsync(id, ct);
        if (existing is null)
            return Result<UpdateRiskConfigurationResponse>.Fail(ErrorType.NotFound, "Risk factor not found");

        existing.Core.UpdateName(request.Core.Name).UpdatePercentage(request.Core.Percentage);
        if (request.Core.IsActive) existing.Core.Activate(); else existing.Core.Deactivate();

        await _risks.UpdateAsync(existing, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateRiskConfigurationResponse(RiskConfigurationCoreDto.From(existing));
        return Result<UpdateRiskConfigurationResponse>.Ok(response);
    }
}
