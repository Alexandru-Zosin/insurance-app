using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class PremiumRule
{
    public int PremiumRuleId { get; set; }

    public Guid PremiumRuleKey { get; set; }

    public string RuleKind { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Percentage { get; set; }

    public bool IsActive { get; set; }

    public string? FeeType { get; set; }

    public DateOnly? EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public int? CityId { get; set; }

    public int? CountyId { get; set; }

    public int? CountryId { get; set; }

    public string? BuildingType { get; set; }

    public int? RiskCategoryId { get; set; }

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual County? County { get; set; }

    public virtual RiskCategory? RiskCategory { get; set; }
}
