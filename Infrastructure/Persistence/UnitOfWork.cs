using Application.Common;
using Infrastructure.Persistence.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork(InsuranceDbContext _dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (SqlServerUniqueViolation(ex))
        {
            throw new UniqueConstraintViolationException(ex);
        }
    }

    private bool SqlServerUniqueViolation(DbUpdateException ex)
    {
        var sql = ex.InnerException as SqlException;
        if (sql is null)
            return false;

        return sql.Number == 2601 || sql.Number == 2627;
    }
}

