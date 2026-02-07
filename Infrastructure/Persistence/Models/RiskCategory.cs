using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class RiskCategory
{
    public int RiskCategoryId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<PremiumRule> PremiumRules { get; set; } = new List<PremiumRule>();

    public virtual ICollection<Building> BuildingKeys { get; set; } = new List<Building>();
}
