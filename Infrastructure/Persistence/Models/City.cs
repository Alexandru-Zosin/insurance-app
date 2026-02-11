using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class City
{
    public int CityId { get; set; }

    public int CountyId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();

    public virtual County County { get; set; } = null!;

    public virtual ICollection<PremiumRule> PremiumRules { get; set; } = new List<PremiumRule>();
}
