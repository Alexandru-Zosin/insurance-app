namespace Application.Services.Metadata.DTOs;

public sealed record UpdateFeeConfigurationRequest(Guid FeeConfigId, FeeConfigurationUpdateDto FeeConfig);
