using Application.Common;
using Application.Repositories;
using Application.Services.Metadata.DTOs;
using Application.Services.Shared.DTOs.MetadataDTOs;
using Domain.Configurations;

public sealed class FeeConfigService(
    IFeeConfigurationRepository _fees,
    IUnitOfWork _uow) : IFeeConfigService
{
    public async Task<Result<CreateFeeConfigResponse>> CreateFeeConfigAsync(
        CreateFeeConfigRequest request,
        CancellationToken ct = default)
    {
        var fee = FeeConfiguration.Create(
            request.FeeConfig.Name,
            request.FeeConfig.Type,
            request.FeeConfig.Percentage,
            request.FeeConfig.ValidityPeriod.ToDomain(),
            request.FeeConfig.IsActive);

        await _fees.AddAsync(fee, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<CreateFeeConfigResponse>.Fail(
                ErrorType.Conflict, "Fee configuration already exists for the given period");
        }

        var response = new CreateFeeConfigResponse(FeeConfigDetailedDto.From(fee));
        return Result<CreateFeeConfigResponse>.Ok(response);
    }

    public async Task<Result<UpdateFeeConfigResponse>> UpdateFeeConfigAsync(
        UpdateFeeConfigRequest request,
        CancellationToken ct = default)
    {
        var fee = await _fees.GetByIdAsync(request.FeeConfigId, ct);
        if (fee == null)
            return Result<UpdateFeeConfigResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        fee.UpdatePercentage(request.FeeConfig.Percentage)
           .UpdateValidity(request.FeeConfig.ValidityPeriod);

        await _fees.UpdateAsync(fee, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateFeeConfigResponse(FeeConfigDetailedDto.From(fee));
        return Result<UpdateFeeConfigResponse>.Ok(response);
    }

    public async Task<Result<SetFeeConfigStatusResponse>> SetFeeConfigStatusAsync(
        SetFeeConfigStatusRequest request,
        CancellationToken ct = default)
    {
        var fee = await _fees.GetByIdAsync(request.FeeConfigId, ct);
        if (fee == null)
            return Result<SetFeeConfigStatusResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        if (request.Active)
            fee.Activate();
        else
            fee.Deactivate();

        await _fees.UpdateAsync(fee, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<SetFeeConfigStatusResponse>.Ok(
            new SetFeeConfigStatusResponse(FeeConfigDetailedDto.From(fee)));
    }

    public async Task<Result<GetFeeConfigResponse>> GetFeeConfigDetailsAsync(
        GetFeeConfigRequest request,
        CancellationToken ct = default)
    {
        var fee = await _fees.GetByIdAsync(request.FeeConfigId, ct);
        if (fee == null)
            return Result<GetFeeConfigResponse>.Fail(
                ErrorType.NotFound, "Fee configuration not found");

        var response = new GetFeeConfigResponse(FeeConfigDetailedDto.From(fee));
        return Result<GetFeeConfigResponse>.Ok(response);
    }

    public async Task<Result<ListFeeConfigsResponse>> ListFeeConfigsAsync(
        ListFeeConfigsRequest request,
        CancellationToken ct = default)
    {
        var list = await _fees.ListAsync(request.OnlyActive, ct);
        var response = new ListFeeConfigsResponse(list.Select(FeeConfigListItemDto.From).ToArray());

        return Result<ListFeeConfigsResponse>.Ok(response);
    }
}