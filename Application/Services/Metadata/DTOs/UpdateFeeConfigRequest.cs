namespace Application.Services.Metadata.DTOs;

public sealed record UpdateFeeConfigRequest(Guid FeeConfigId, FeeConfigUpdateDto FeeConfig);
