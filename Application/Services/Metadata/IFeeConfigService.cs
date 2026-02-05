using Application.Common;
using Application.Services.Metadata.DTOs;

public interface IFeeConfigService
{
    Task<Result<CreateFeeConfigResponse>> CreateFeeConfigAsync(CreateFeeConfigRequest request, CancellationToken ct = default);
    Task<Result<GetFeeConfigResponse>> GetFeeConfigDetailsAsync(GetFeeConfigRequest request, CancellationToken ct = default);
    Task<Result<ListFeeConfigsResponse>> ListFeeConfigsAsync(ListFeeConfigsRequest request, CancellationToken ct = default);
    Task<Result<SetFeeConfigStatusResponse>> SetFeeConfigStatusAsync(SetFeeConfigStatusRequest request, CancellationToken ct = default);
    Task<Result<UpdateFeeConfigResponse>> UpdateFeeConfigAsync(UpdateFeeConfigRequest request, CancellationToken ct = default);
}