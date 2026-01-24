using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Broker
{
    public int BrokerId { get; set; }

    public Guid BrokerKey { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
