using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Building
{
    public int BuildingId { get; set; }

    public Guid BuildingKey { get; set; }

    public Guid OwnerClientId { get; set; }

    public int CityId { get; set; }

    public string Street { get; set; } = null!;

    public string Number { get; set; } = null!;

    public int ConstructionYear { get; set; }

    public string BuildingType { get; set; } = null!;

    public int SurfaceArea { get; set; }

    public decimal InsuredValueAmount { get; set; }

    public string InsuredValueCurrencyCode { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual Currency InsuredValueCurrencyCodeNavigation { get; set; } = null!;

    public virtual Client OwnerClient { get; set; } = null!;

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();

    public virtual ICollection<RiskCategory> RiskCategories { get; set; } = new List<RiskCategory>();
}
