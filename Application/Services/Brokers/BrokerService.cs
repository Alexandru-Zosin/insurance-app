using Application.Common;
using Application.Repositories;
using Application.Services.Brokers.DTOs;
using Application.Services.Shared.DTOs.BrokerDTOs;
using Domain.Brokers;

namespace Application.Services.Brokers;

public sealed class BrokerService(
    IBrokerRepository _brokers,
    IUnitOfWork _uow) : IBrokerService
{
    public async Task<Result<CreateBrokerResponse>> CreateBrokerAsync(
        CreateBrokerRequest request,
        CancellationToken ct = default)
    {
        var broker = Broker.Create(
            request.Broker.BrokerCode,
            request.Broker.Name,
            request.Broker.ContactInfo.ToDomain(),
            request.Broker.IsActive,
            request.Broker.CommissionPercentage);

        _brokers.Add(broker, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<CreateBrokerResponse>.Fail(ErrorType.Conflict, "Broker code already exists");
        }

        var response = new CreateBrokerResponse(BrokerDetailedDto.From(broker));
        return Result<CreateBrokerResponse>.Ok(response);
    }

    public async Task<Result<UpdateBrokerResponse>> UpdateBrokerAsync(
        Guid requestBrokerId,
        UpdateBrokerRequest request,
        CancellationToken ct = default)
    {
        var broker = await _brokers.GetByIdAsync(requestBrokerId, ct);
        if (broker == null)
            return Result<UpdateBrokerResponse>.Fail(ErrorType.NotFound, "Broker not found");

        var newName = request.Broker.Name;
        var newContactInfo = request.Broker.ContactInfo.ToDomain();
        var newCommissionPercentage = request.Broker.CommissionPercentage;

        broker.UpdateName(newName)
              .UpdateContactInfo(newContactInfo)
              .UpdateCommissionPercentage(newCommissionPercentage);

        await _brokers.UpdateAsync(broker, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateBrokerResponse(BrokerDetailedDto.From(broker));
        return Result<UpdateBrokerResponse>.Ok(response);
    }

    public async Task<Result<SetBrokerStatusResponse>> SetBrokerStatusAsync(
        SetBrokerStatusRequest request,
        CancellationToken ct = default)
    {
        var broker = await _brokers.GetByIdAsync(request.BrokerId, ct);
        if (broker == null)
            return Result<SetBrokerStatusResponse>.Fail(ErrorType.NotFound, "Broker not found");

        if (request.Active)
            broker.Activate();
        else
            broker.Deactivate();

        await _brokers.UpdateAsync(broker, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new SetBrokerStatusResponse(BrokerDetailedDto.From(broker));
        return Result<SetBrokerStatusResponse>.Ok(response);
    }

    public async Task<Result<GetBrokerDetailsResponse>> GetBrokerDetailsAsync(
        GetBrokerDetailsRequest request,
        CancellationToken ct = default)
    {
        var broker = await _brokers.GetByIdAsync(request.BrokerId, ct);
        if (broker == null)
            return Result<GetBrokerDetailsResponse>.Fail(ErrorType.NotFound, "Broker not found");

        var response = new GetBrokerDetailsResponse(BrokerDetailedDto.From(broker));
        return Result<GetBrokerDetailsResponse>.Ok(response);
    }

    public async Task<Result<ListBrokersResponse>> ListBrokersAsync(
        ListBrokersRequest request,
        CancellationToken ct = default)
    {
        var list = await _brokers.ListAsync(request.Page, ct);

        var response = new ListBrokersResponse(list.Select(BrokerListItemDto.From).ToArray());
        return Result<ListBrokersResponse>.Ok(response);
    }
}
