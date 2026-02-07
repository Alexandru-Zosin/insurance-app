using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<County> Counties { get; set; } = new List<County>();

    public virtual ICollection<PremiumRule> PremiumRules { get; set; } = new List<PremiumRule>();
}
