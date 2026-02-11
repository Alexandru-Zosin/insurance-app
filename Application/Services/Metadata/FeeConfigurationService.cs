using Application.Common;
using Application.Repositories;
using Application.Services.Metadata.DTOs;
using Application.Services.Shared.DTOs.MetadataDTOs;
using Domain.Configurations;

public sealed class FeeConfigurationService(
    IFeeConfigurationRepository feeConfigurationRepository,
    IUnitOfWork uow) : IFeeConfigurationService
{
    public async Task<Result<CreateFeeConfigurationResponse>> CreateFeeConfigurationAsync(
        CreateFeeConfigurationRequest request,
        CancellationToken ct = default)
    {
        var newFeeConfiguration = FeeConfiguration.Create(
            request.FeeConfig.Name,
            request.FeeConfig.Type,
            request.FeeConfig.Percentage,
            request.FeeConfig.ValidityPeriod.MapToDomain(),
            request.FeeConfig.IsActive);

        feeConfigurationRepository.Add(newFeeConfiguration);
        await uow.SaveChangesAsync(ct);
       
        var response = new CreateFeeConfigurationResponse(FeeConfigurationDetailedDto.From(newFeeConfiguration));
        return Result<CreateFeeConfigurationResponse>.Ok(response);
    }

    public async Task<Result<UpdateFeeConfigurationResponse>> UpdateFeeConfigurationAsync(
        Guid feeConfigurationId,
        UpdateFeeConfigurationRequest request,
        CancellationToken ct = default)
    {
        var feeConfiguration = await feeConfigurationRepository.GetByIdAsync(feeConfigurationId, ct);
        if (feeConfiguration == null)
            return Result<UpdateFeeConfigurationResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        var updatedPercentage = request.FeeConfig.Percentage;
        var updatedValidity = request.FeeConfig.ValidityPeriod;

        feeConfiguration.UpdatePercentage(updatedPercentage)
                        .UpdateValidity(updatedValidity);

        await feeConfigurationRepository.UpdateAsync(feeConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new UpdateFeeConfigurationResponse(FeeConfigurationDetailedDto.From(feeConfiguration));
        return Result<UpdateFeeConfigurationResponse>.Ok(response);
    }

    public async Task<Result<SetFeeConfigStatusResponse>> SetFeeConfigurationStatusAsync(
        Guid feeConfigurationId,
        bool isActive,
        CancellationToken ct = default)
    {
        var feeConfiguration = await feeConfigurationRepository.GetByIdAsync(feeConfigurationId, ct);
        if (feeConfiguration == null)
            return Result<SetFeeConfigStatusResponse>.Fail(ErrorType.NotFound, "Fee configuration not found");

        if (isActive)
            feeConfiguration.Activate();
        else
            feeConfiguration.Deactivate();

        await feeConfigurationRepository.UpdateAsync(feeConfiguration, ct);
        await uow.SaveChangesAsync(ct);

        var response = new SetFeeConfigStatusResponse(FeeConfigurationDetailedDto.From(feeConfiguration));
        return Result<SetFeeConfigStatusResponse>.Ok(response);
    }

    public async Task<Result<ListFeeConfigurationsResponse>> ListFeeConfigurationsAsync(
        CancellationToken ct = default)
    {
        var feeConfigurations = await feeConfigurationRepository.ListAsync(ct);

        var response = new ListFeeConfigurationsResponse(feeConfigurations.Select(FeeConfigurationListDto.From).ToArray());
        return Result<ListFeeConfigurationsResponse>.Ok(response);
    }
}