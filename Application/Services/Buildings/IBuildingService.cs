using Application.Services.Buildings.DTOs;
using Application.Common;

namespace Application.Services.Buildings;

public interface IBuildingService
{
    Task<Result<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(Guid buildingId, CancellationToken ct = default);
    Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(Guid clientId, CancellationToken ct = default);
    Task<Result<RegisterBuildingResponse>> RegisterBuildingAsync(RegisterBuildingRequest request, CancellationToken ct = default);
    Task<Result<UpdateBuildingResponse>> UpdateBuildingAsync(Guid buildingId, UpdateBuildingRequest request, CancellationToken ct = default);
}