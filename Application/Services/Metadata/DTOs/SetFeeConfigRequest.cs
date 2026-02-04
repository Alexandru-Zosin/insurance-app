namespace Application.Services.Metadata.DTOs;

public sealed record SetFeeConfigStatusRequest(Guid FeeConfigId, bool Active);
