using Application.Common;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.UnitOfWork;

public sealed class UnitOfWork(InsuranceDbContext _dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (SqlServerErrors.IsUniqueViolation(ex))
        {
            throw new UniqueConstraintViolationException(ex);
        }
    }
}

