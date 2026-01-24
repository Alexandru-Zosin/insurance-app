using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Building
{
    public int BuildingId { get; set; }

    public Guid BuildingKey { get; set; }

    public int ClientId { get; set; }

    public int CityId { get; set; }

    public int ConstructionYear { get; set; }

    public string Address { get; set; } = null!;

    public string BuildingType { get; set; } = null!;

    public int NumberOfFloors { get; set; }

    public int SurfaceArea { get; set; }

    public int InsuredValue { get; set; }

    public int? FloodRiskZone { get; set; }

    public int? EarthquakeRiskZone { get; set; }

    public string InsuredValueCurrency { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
