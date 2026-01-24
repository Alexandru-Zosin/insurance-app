using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class County
{
    public int CountyId { get; set; }

    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;
}
