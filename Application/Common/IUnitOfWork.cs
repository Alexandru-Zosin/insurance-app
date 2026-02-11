namespace Application.Common;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
    void EnqueueAudit(AuditEntry entry);
}
