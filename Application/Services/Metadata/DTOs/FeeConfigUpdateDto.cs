using Domain.Shared;

namespace Application.Services.Metadata.DTOs;

public sealed record FeeConfigUpdateDto(decimal Percentage, ValidityPeriod ValidityPeriod);
