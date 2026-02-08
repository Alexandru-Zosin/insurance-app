namespace Application.Common;
public sealed record AuditEntry(
    string EntityType,
    Guid EntityId,
    string Action,
    string? OldValue,
    string? NewValue,
    Guid PerformedBy,
    DateTime PerformedAtUtc
);
