namespace Domain.Configurations;

public interface IRiskConfiguration : IPremiumRule
{
    RiskConfigCore Core { get; }
}