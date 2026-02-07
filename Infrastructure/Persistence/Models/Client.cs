using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public Guid ClientKey { get; set; }

    public string ClientType { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string IdentificationNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Street { get; set; }

    public string? Number { get; set; }

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
