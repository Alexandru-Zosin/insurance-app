using Application.Common;
using Application.Services.Metadata.DTOs;

namespace Application.Services.Risks
{
    public interface IRiskConfigurationService
    {
        Task<Result<CreateRiskConfigurationResponse>> CreateBuildingTypeAsync(CreateBuildingTypeRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateCityAsync(CreateCityRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateCountryAsync(CreateCountryRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateCountyAsync(CreateCountyRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateZoneCategoryAsync(CreateZoneCategoryRiskRequest request, CancellationToken ct = default);
        Task<Result<ListRiskConfigurationsResponse>> ListAsync(CancellationToken ct = default);
        Task<Result<UpdateRiskConfigurationResponse>> UpdateCoreAsync(Guid riskConfigurationId, UpdateRiskConfigurationRequest request, CancellationToken ct = default);
    }
}