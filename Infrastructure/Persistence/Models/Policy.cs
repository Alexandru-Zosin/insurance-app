using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Policy
{
    public int PolicyId { get; set; }

    public Guid PolicyNumber { get; set; }

    public Guid ClientKey { get; set; }

    public Guid BuildingKey { get; set; }

    public Guid BrokerKey { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal BasePremiumAmount { get; set; }

    public decimal FinalPremiumAmount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public DateOnly? LastUpdateDate { get; set; }

    public string? CancellationReason { get; set; }

    public DateOnly? CancellationEffectiveDate { get; set; }

    public virtual Broker BrokerKeyNavigation { get; set; } = null!;

    public virtual Building BuildingKeyNavigation { get; set; } = null!;

    public virtual Client ClientKeyNavigation { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;
}
