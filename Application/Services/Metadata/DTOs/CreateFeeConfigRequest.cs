using Application.Services.Shared.DTOs.MetadataDTOs;

namespace Application.Services.Metadata.DTOs
{
    public sealed record CreateFeeConfigRequest(
    FeeConfigCoreDto FeeConfig);
}
