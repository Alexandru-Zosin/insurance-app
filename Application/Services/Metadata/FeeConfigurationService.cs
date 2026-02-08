using Application.Common;
using Application.Repositories;
using Application.Services.Metadata.DTOs;
using Application.Services.Shared.DTOs.MetadataDTOs;
using Domain.Configurations;

public sealed class FeeConfigurationService(
    IFeeConfigurationRepository _fees,
    IUnitOfWork _uow) : IFeeConfigurationService 
{
    public async Task<Result<CreateFeeConfigurationResponse>> CreateFeeConfigAsync(
        CreateFeeConfigurationRequest request,
        CancellationToken ct = default)
    {
        var fee = FeeConfiguration.Create(
            request.FeeConfig.Name,
            request.FeeConfig.Type,
            request.FeeConfig.Percentage,
            request.FeeConfig.ValidityPeriod.ToDomain(),
            request.FeeConfig.IsActive);

        _fees.Add(fee, ct);
        await _uow.SaveChangesAsync(ct);
       
        var response = new CreateFeeConfigurationResponse(FeeConfigurationDetailedDto.From(fee));
        return Result<CreateFeeConfigurationResponse>.Ok(response);
    }

    public async Task<Result<UpdateFeeConfigurationResponse>> UpdateFeeConfigAsync(
        Guid requestFeeConfigId,
        UpdateFeeConfigurationRequest request,
        CancellationToken ct = default)
    {
        var fee = await _fees.GetByIdAsync(requestFeeConfigId, ct);
        if (fee == null)
            return Result<UpdateFeeConfigurationResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        fee.UpdatePercentage(request.FeeConfig.Percentage)
           .UpdateValidity(request.FeeConfig.ValidityPeriod);

        await _fees.UpdateAsync(fee, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateFeeConfigurationResponse(FeeConfigurationDetailedDto.From(fee));
        return Result<UpdateFeeConfigurationResponse>.Ok(response);
    }

    public async Task<Result<SetFeeConfigStatusResponse>> SetFeeConfigStatusAsync(
        Guid requestFeeConfigId,
        SetFeeConfigStatusRequest request,
        CancellationToken ct = default)
    {
        var fee = await _fees.GetByIdAsync(requestFeeConfigId, ct);
        if (fee == null)
            return Result<SetFeeConfigStatusResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        if (request.Active)
            fee.Activate();
        else
            fee.Deactivate();

        await _fees.UpdateAsync(fee, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new SetFeeConfigStatusResponse(FeeConfigurationDetailedDto.From(fee));
        return Result<SetFeeConfigStatusResponse>.Ok(response);
    }

    public async Task<Result<GetFeeConfigurationResponse>> GetFeeConfigDetailsAsync(
        GetFeeConfigRequest request,
        CancellationToken ct = default)
    {
        var fee = await _fees.GetByIdAsync(request.FeeConfigId, ct);
        if (fee == null)
            return Result<GetFeeConfigurationResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        var response = new GetFeeConfigurationResponse(FeeConfigurationDetailedDto.From(fee));
        return Result<GetFeeConfigurationResponse>.Ok(response);
    }

    public async Task<Result<ListFeeConfigurationsResponse>> ListFeeConfigsAsync(
        ListFeeConfigurationsRequest request,
        CancellationToken ct = default)
    {
        var list = await _fees.ListAsync(ct);

        var response = new ListFeeConfigurationsResponse(list.Select(FeeConfigurationListDto.From).ToArray());
        return Result<ListFeeConfigurationsResponse>.Ok(response);
    }
}