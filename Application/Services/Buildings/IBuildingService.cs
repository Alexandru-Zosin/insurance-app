using Application.Services.Buildings.DTOs;
using Domain.Common;

namespace Application.Services.Buildings;

public interface IBuildingService
{
    Task<Result<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(GetBuildingDetailsRequest request, CancellationToken ct = default);
    Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(GetBuildingsForClientRequest request, CancellationToken ct = default);
    Task<Result<RegisterBuildingResponse>> RegisterBuildingAsync(RegisterBuildingRequest request, CancellationToken ct = default);
    Task<Result<UpdateBuildingResponse>> UpdateBuildingAsync(UpdateBuildingRequest request, CancellationToken ct = default);
}