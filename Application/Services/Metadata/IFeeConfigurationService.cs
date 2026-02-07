using Application.Common;
using Application.Services.Metadata.DTOs;

public interface IFeeConfigurationService
{
    Task<Result<CreateFeeConfigurationResponse>> CreateFeeConfigAsync(CreateFeeConfigurationRequest request, CancellationToken ct = default);
    Task<Result<GetFeeConfigurationResponse>> GetFeeConfigDetailsAsync(GetFeeConfigRequest request, CancellationToken ct = default);
    Task<Result<ListFeeConfigurationsResponse>> ListFeeConfigsAsync(ListFeeConfigurationsRequest request, CancellationToken ct = default);
    Task<Result<SetFeeConfigStatusResponse>> SetFeeConfigStatusAsync(Guid feeConfigId, SetFeeConfigStatusRequest request, CancellationToken ct = default);
    Task<Result<UpdateFeeConfigurationResponse>> UpdateFeeConfigAsync(Guid feeConfigId, UpdateFeeConfigurationRequest request, CancellationToken ct = default);
}