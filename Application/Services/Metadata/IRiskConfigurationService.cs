using Application.Common;
using Application.Services.Metadata.DTOs;

namespace Application.Services.Risks
{
    public interface IRiskConfigurationService
    {
        Task<Result<CreateRiskConfigurationResponse>> CreateBuildingTypeRiskConfigurationAsync(CreateBuildingTypeRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateCityRiskConfigurationAsync(CreateCityRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateCountryRiskConfigurationAsync(CreateCountryRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateCountyRiskConfigurationAsync(CreateCountyRiskRequest request, CancellationToken ct = default);
        Task<Result<CreateRiskConfigurationResponse>> CreateZoneCategoryRiskConfigurationAsync(CreateZoneCategoryRiskRequest request, CancellationToken ct = default);
        Task<Result<ListRiskConfigurationsResponse>> ListRiskConfigurationsAsync(CancellationToken ct = default);
        Task<Result<UpdateRiskConfigurationResponse>> UpdateRiskConfigurationAsync(Guid riskConfigurationId, UpdateRiskConfigurationRequest request, CancellationToken ct = default);
    }
}