using Application.Common;
using Application.Repositories;
using Application.Services.Currencies.DTOs;
using Application.Services.Shared.DTOs.CurrencyDTOs;
using Domain.Currencies;

namespace Application.Services.Currencies;

public sealed class CurrencyService(
    ICurrencyRepository _currencyRepository,
    IUnitOfWork _uow) : ICurrencyService
{
    public async Task<Result<AddCurrencyResponse>> AddCurrencyAsync(
        AddCurrencyRequest request,
        CancellationToken ct = default)
    {
        var newCurrency = Currency.Create(
            request.Currency.Code,
            request.Currency.Name,
            request.Currency.ExchangeRateToBase,
            request.Currency.IsActive);

        _currencyRepository.Add(newCurrency, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<AddCurrencyResponse>.Fail(
                ErrorType.Conflict, "Currency code already exists");
        }

        return Result<AddCurrencyResponse>.Ok(new AddCurrencyResponse(CurrencyDto.From(newCurrency)));
    }

    public async Task<Result<UpdateCurrencyResponse>> UpdateCurrencyAsync(
        string currencyCode,
        UpdateCurrencyRequest request,
        CancellationToken ct = default)
    {
        var currency = await _currencyRepository.GetByCodeAsync(currencyCode, ct);
        if (currency == null)
            return Result<UpdateCurrencyResponse>.Fail(ErrorType.NotFound, "Currency not found");

        var updatedExchangeRateToBase = request.Currency.ExchangeRateToBase;
        currency.UpdateExchangeRateToBase(updatedExchangeRateToBase);

        await _currencyRepository.UpdateAsync(currency, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateCurrencyResponse>.Ok(
            new UpdateCurrencyResponse(CurrencyDto.From(currency)));
    }

    public async Task<Result<SetCurrencyStatusResponse>> SetCurrencyStatusAsync(
        string currencyCode,
        bool isActive,
        CancellationToken ct = default)
    {
        var currency = await _currencyRepository.GetByCodeAsync(currencyCode, ct);
        if (currency == null)
            return Result<SetCurrencyStatusResponse>.Fail(
                ErrorType.NotFound, "Currency not found");

        if (isActive)
            currency.Activate();
        else
            currency.Deactivate();

        await _currencyRepository.UpdateAsync(currency, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<SetCurrencyStatusResponse>.Ok(
            new SetCurrencyStatusResponse(CurrencyDto.From(currency)));
    }

    public async Task<Result<ListCurrenciesResponse>> ListCurrenciesAsync(CancellationToken ct = default)
    {
        var currencies = await _currencyRepository.ListAsync(ct);
        var response = new ListCurrenciesResponse(currencies.Select(CurrencyDto.From).ToList());

        return Result<ListCurrenciesResponse>.Ok(response);
    }
}