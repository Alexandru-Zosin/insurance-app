using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Currency
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal ExchangeRateToBase { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
