using Domain.Configurations;

namespace Application.Services.Metadata.DTOs;

public sealed record CreateRiskRequest<TCfg>(TCfg RiskConfig) where TCfg : RiskFactorConfiguration<TCfg>;