using Domain.Shared;

namespace Application.Services.Metadata.DTOs;

public sealed record FeeConfigurationUpdateDto(decimal Percentage, ValidityPeriod ValidityPeriod);
