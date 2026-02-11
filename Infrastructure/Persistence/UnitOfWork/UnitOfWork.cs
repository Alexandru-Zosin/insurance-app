using Application.Common;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.UnitOfWork;

public sealed class UnitOfWork(InsuranceDbContext dbContext) : IUnitOfWork
{
    private readonly List<AuditEntry> _pendingAudits = new();

    private void FlushAuditsToDbContext()
    {
        if (_pendingAudits.Count == 0) return;

        foreach (var entry in _pendingAudits)
        {
            dbContext.Set<AuditLog>().Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = entry.EntityType,
                EntityId = entry.EntityId,
                Action = entry.Action,
                OldValue = entry.OldValue,
                NewValue = entry.NewValue,
                PerformedBy = entry.PerformedBy,
                PerformedAtUtc = entry.PerformedAtUtc
            });
        }
    }

    public void EnqueueAudit(AuditEntry entry)
    {
        _pendingAudits.Add(entry);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            FlushAuditsToDbContext();
            await dbContext.SaveChangesAsync();
            _pendingAudits.Clear();
        }
        catch (DbUpdateException ex) when (SqlServerErrors.IsUniqueViolation(ex))
        {
            throw new UniqueConstraintViolationException(ex);
        }
    }
}

