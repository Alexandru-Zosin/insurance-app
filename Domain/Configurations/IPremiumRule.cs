using Domain.Policies;

namespace Domain.Configurations;

public interface IPremiumRule
{
    decimal Percentage { get; }
    bool IsApplicable(PolicyDraftContext ctx);
}
