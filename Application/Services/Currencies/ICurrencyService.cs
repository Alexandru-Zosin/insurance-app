using Application.Common;
using Application.Services.Currencies.DTOs;

namespace Application.Services.Currencies
{
    public interface ICurrencyService
    {
        Task<Result<AddCurrencyResponse>> AddCurrencyAsync(AddCurrencyRequest request, CancellationToken ct = default);
        Task<Result<ListCurrenciesResponse>> ListCurrenciesAsync(ListCurrenciesRequest request, CancellationToken ct = default);
        Task<Result<SetCurrencyStatusResponse>> SetCurrencyStatusAsync(SetCurrencyStatusRequest request, CancellationToken ct = default);
        Task<Result<UpdateCurrencyResponse>> UpdateCurrencyAsync(UpdateCurrencyRequest request, CancellationToken ct = default);
    }
}