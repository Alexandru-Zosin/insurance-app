using Application.Common;
using Application.Repositories;
using Domain.Brokers;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfBroker = Infrastructure.Persistence.Models.Broker;

namespace Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository(InsuranceDbContext dbContext) : IBrokerRepository
{
    public void Add(Broker brokerToAdd, CancellationToken ct = default)
    {
        var newBrokerRow = MapToEf(brokerToAdd);

        dbContext.Brokers.Add(newBrokerRow);
    }

    public async Task UpdateAsync(Broker updatedBroker, CancellationToken ct = default)
    {
        var existingBrokerRow = await dbContext.Brokers
            .SingleOrDefaultAsync(b => b.BrokerKey == updatedBroker.Id, ct);
        
        if (existingBrokerRow != null)
            MapOntoEf(existingBrokerRow, updatedBroker);
    }

    public async Task<Broker?> GetByIdAsync(Guid brokerId, CancellationToken ct = default)
    {
        var brokerRow = await dbContext.Brokers
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.BrokerKey == brokerId, ct);

        return brokerRow is null ? null : MapMapToDomain(brokerRow);
    }

    public async Task<Broker?> GetByCodeAsync(string brokerCode, CancellationToken ct = default)
    {
        var normalizedBrokerCode = brokerCode.Trim();

        var brokerRow = await dbContext.Brokers
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.Code == normalizedBrokerCode, ct);

        return brokerRow is null ? null : MapMapToDomain(brokerRow);
    }

    public async Task<IReadOnlyList<Broker>> ListAsync(PageRequest page, CancellationToken ct = default)
    {
        var brokerRows = await dbContext.Brokers
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ThenBy(b => b.Code)
            .Skip(page.Offset)
            .Take(page.Take)
            .ToListAsync(ct);

        return brokerRows.Select(MapMapToDomain).ToList();
    }

    private static EfBroker MapToEf(Broker domain)
    {
        return new EfBroker
        {
            BrokerKey = domain.Id,
            Code = domain.Code,
            Name = domain.Name,
            Email = domain.ContactInfo.Email,
            Phone = domain.ContactInfo.Phone,
            IsActive = domain.IsActive,
            CommissionPercentage = domain.CommissionPercentage
        };
    }

    private static void MapOntoEf(EfBroker ef, Broker domain)
    {
        ef.Code = domain.Code;
        ef.Name = domain.Name;
        ef.Email = domain.ContactInfo.Email;
        ef.Phone = domain.ContactInfo.Phone;
        ef.IsActive = domain.IsActive;
        ef.CommissionPercentage = domain.CommissionPercentage;
    }

    private static Broker MapMapToDomain(EfBroker ef)
    {
        return Broker.Rehydrate(
            id: ef.BrokerKey,
            code: ef.Code,
            name: ef.Name,
            contactInfo: ContactInfo.Create(ef.Email, ef.Phone),
            isActive: ef.IsActive,
            commissionPercentage: ef.CommissionPercentage);
    }
}
