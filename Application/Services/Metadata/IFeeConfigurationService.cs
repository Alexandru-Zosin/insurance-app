using Application.Common;
using Application.Services.Metadata.DTOs;

public interface IFeeConfigurationService
{
    Task<Result<CreateFeeConfigurationResponse>> CreateFeeConfigurationAsync(CreateFeeConfigurationRequest request, CancellationToken ct = default);
    Task<Result<ListFeeConfigurationsResponse>> ListFeeConfigurationsAsync(CancellationToken ct = default);
    Task<Result<SetFeeConfigStatusResponse>> SetFeeConfigurationStatusAsync(Guid feeConfigId, bool request, CancellationToken ct = default);
    Task<Result<UpdateFeeConfigurationResponse>> UpdateFeeConfigurationAsync(Guid feeConfigId, UpdateFeeConfigurationRequest request, CancellationToken ct = default);
}