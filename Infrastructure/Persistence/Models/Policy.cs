using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Policy
{
    public int PolicyId { get; set; }

    public Guid PolicyKey { get; set; }

    public int ClientId { get; set; }

    public int BuildingId { get; set; }

    public int BrokerId { get; set; }

    public decimal PremiumAmount { get; set; }

    public string PremiumCurrency { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Broker Broker { get; set; } = null!;

    public virtual Building Building { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;
}
