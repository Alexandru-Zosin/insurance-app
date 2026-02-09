using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class AuditLog
{
    public Guid Id { get; set; }

    public string EntityType { get; set; } = null!;

    public Guid EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public Guid PerformedBy { get; set; }

    public DateTime PerformedAtUtc { get; set; }
}
