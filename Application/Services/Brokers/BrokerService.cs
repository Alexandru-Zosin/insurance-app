using Application.Common;
using Application.Repositories;
using Application.Services.Brokers.DTOs;
using Application.Services.Shared.DTOs.BrokerDTOs;
using Domain.Brokers;

namespace Application.Services.Brokers;

public sealed class BrokerService(
    IBrokerRepository _brokerRepository,
    IUnitOfWork _uow) : IBrokerService
{
    public async Task<Result<CreateBrokerResponse>> CreateBrokerAsync(
        CreateBrokerRequest request,
        CancellationToken ct = default)
    {
        var newBroker = Broker.Create(
            request.Broker.BrokerCode,
            request.Broker.Name,
            request.Broker.ContactInfo.MapToDomain(),
            request.Broker.IsActive,
            request.Broker.CommissionPercentage);

        _brokerRepository.Add(newBroker, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<CreateBrokerResponse>.Fail(ErrorType.Conflict, "Broker code already exists");
        }

        var response = new CreateBrokerResponse(BrokerDetailedDto.From(newBroker));
        return Result<CreateBrokerResponse>.Ok(response);
    }

    public async Task<Result<UpdateBrokerResponse>> UpdateBrokerAsync(
        Guid brokerId,
        UpdateBrokerRequest request,
        CancellationToken ct = default)
    {
        var broker = await _brokerRepository.GetByIdAsync(brokerId, ct);
        if (broker == null)
            return Result<UpdateBrokerResponse>.Fail(ErrorType.NotFound, "Broker not found");

        var updatedName = request.Broker.Name;
        var updatedContactInfo = request.Broker.ContactInfo.MapToDomain();
        var updatedCommissionPercentage = request.Broker.CommissionPercentage;

        broker.UpdateName(updatedName)
              .UpdateContactInfo(updatedContactInfo)
              .UpdateCommissionPercentage(updatedCommissionPercentage);

        await _brokerRepository.UpdateAsync(broker, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateBrokerResponse(BrokerDetailedDto.From(broker));
        return Result<UpdateBrokerResponse>.Ok(response);
    }

    public async Task<Result<SetBrokerStatusResponse>> SetBrokerStatusAsync(
        Guid brokerId,
        bool isActive,
        CancellationToken ct = default)
    {
        var broker = await _brokerRepository.GetByIdAsync(brokerId, ct);
        if (broker == null)
            return Result<SetBrokerStatusResponse>.Fail(ErrorType.NotFound, "Broker not found");

        if (isActive)
            broker.Activate();
        else
            broker.Deactivate();

        await _brokerRepository.UpdateAsync(broker, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new SetBrokerStatusResponse(BrokerDetailedDto.From(broker));
        return Result<SetBrokerStatusResponse>.Ok(response);
    }

    public async Task<Result<GetBrokerDetailsResponse>> GetBrokerDetailsAsync(
        Guid brokerId,
        CancellationToken ct = default)
    {
        var broker = await _brokerRepository.GetByIdAsync(brokerId, ct);
        if (broker == null)
            return Result<GetBrokerDetailsResponse>.Fail(ErrorType.NotFound, "Broker not found");

        var response = new GetBrokerDetailsResponse(BrokerDetailedDto.From(broker));
        return Result<GetBrokerDetailsResponse>.Ok(response);
    }

    public async Task<Result<ListBrokersResponse>> ListBrokersAsync(
        ListBrokersRequest request,
        CancellationToken ct = default)
    {
        var brokersPage = await _brokerRepository.ListAsync(request.Page, ct);

        var response = new ListBrokersResponse(brokersPage.Select(BrokerListItemDto.From).ToArray());
        return Result<ListBrokersResponse>.Ok(response);
    }
}
