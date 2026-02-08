using Application.Common;
using Application.Repositories;
using Domain.Brokers;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfBroker = Infrastructure.Persistence.Models.Broker;

namespace Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository(InsuranceDbContext _db) : IBrokerRepository
{
    public void Add(Broker broker, CancellationToken cancellationToken = default)
    {
        if (broker is null) throw new ArgumentNullException(nameof(broker));

        var ef = ToEfModel(broker);

        _db.Brokers.Add(ef);
    }

    public async Task UpdateAsync(Broker broker, CancellationToken cancellationToken = default)
    {
        var ef = await _db.Brokers
            .SingleOrDefaultAsync(b => b.BrokerKey == broker.Id, cancellationToken)
            .ConfigureAwait(false);
        
        UpdateEfModel(ef!, broker);
    }

    public async Task<Broker?> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default)
    {
        if (brokerId == Guid.Empty) throw new ArgumentException("Broker id is required.", nameof(brokerId));

        var ef = await _db.Brokers
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.BrokerKey == brokerId, cancellationToken)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<Broker?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));

        var normalized = code.Trim();

        var ef = await _db.Brokers
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.Code == normalized, ct)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<Broker>> ListAsync(PageRequest pageRequest, CancellationToken ct = default)
    {
        if (pageRequest is null) throw new ArgumentNullException(nameof(pageRequest));

        var skip = (pageRequest.PageNumber - 1) * pageRequest.PageSize;

        var items = await _db.Brokers
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ThenBy(b => b.Code)
            .Skip(skip)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return items.Select(ToDomain).ToList();
    }

    private static EfBroker ToEfModel(Broker domain)
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

    private static void UpdateEfModel(EfBroker ef, Broker domain)
    {
        ef.Code = domain.Code;
        ef.Name = domain.Name;
        ef.Email = domain.ContactInfo.Email;
        ef.Phone = domain.ContactInfo.Phone;
        ef.IsActive = domain.IsActive;
        ef.CommissionPercentage = domain.CommissionPercentage;
    }

    private static Broker ToDomain(EfBroker ef)
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
