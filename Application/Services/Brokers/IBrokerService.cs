using Application.Common;
using Application.Services.Brokers.DTOs;

namespace Application.Services.Brokers
{
    public interface IBrokerService
    {
        Task<Result<CreateBrokerResponse>> CreateBrokerAsync(CreateBrokerRequest request, CancellationToken ct = default);
        Task<Result<GetBrokerDetailsResponse>> GetBrokerDetailsAsync(Guid brokerId, CancellationToken ct = default);
        Task<Result<ListBrokersResponse>> ListBrokersAsync(ListBrokersRequest request, CancellationToken ct = default);
        Task<Result<SetBrokerStatusResponse>> SetBrokerStatusAsync(Guid brokerId, bool request, CancellationToken ct = default);
        Task<Result<UpdateBrokerResponse>> UpdateBrokerAsync(Guid brokerId, UpdateBrokerRequest request, CancellationToken ct = default);
    }
}