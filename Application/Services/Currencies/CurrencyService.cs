using Application.Common;
using Application.Repositories;
using Application.Services.Currencies.DTOs;
using Application.Services.Shared.DTOs.CurrencyDTOs;
using Domain.Currencies;

namespace Application.Services.Currencies;

public sealed class CurrencyService(
    ICurrencyRepository _currencies,
    IUnitOfWork _uow) : ICurrencyService
{
    public async Task<Result<AddCurrencyResponse>> AddCurrencyAsync(
        AddCurrencyRequest request,
        CancellationToken ct = default)
    {
        var currency = Currency.Create(
            request.Currency.Code,
            request.Currency.Name,
            request.Currency.ExchangeRateToBase,
            request.Currency.IsActive);

        _currencies.Add(currency, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<AddCurrencyResponse>.Fail(
                ErrorType.Conflict, "Currency code already exists");
        }

        return Result<AddCurrencyResponse>.Ok(new AddCurrencyResponse(CurrencyDto.From(currency)));
    }

    public async Task<Result<UpdateCurrencyResponse>> UpdateCurrencyAsync(
        string requestCurrencyCode,
        UpdateCurrencyRequest request,
        CancellationToken ct = default)
    {
        var currency = await _currencies.GetByCodeAsync(requestCurrencyCode, ct);
        if (currency == null)
            return Result<UpdateCurrencyResponse>.Fail(ErrorType.NotFound, "Currency not found");

        var newExchangeRateToBase = request.Currency.ExchangeRateToBase;
        currency.UpdateExchangeRateToBase(newExchangeRateToBase);

        await _currencies.UpdateAsync(currency, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateCurrencyResponse>.Ok(
            new UpdateCurrencyResponse(CurrencyDto.From(currency)));
    }

    public async Task<Result<SetCurrencyStatusResponse>> SetCurrencyStatusAsync(
        string requestCurrencyCode,
        SetCurrencyStatusRequest request,
        CancellationToken ct = default)
    {
        var currency = await _currencies.GetByCodeAsync(requestCurrencyCode, ct);
        if (currency == null)
            return Result<SetCurrencyStatusResponse>.Fail(
                ErrorType.NotFound, "Currency not found");

        if (request.Active)
            currency.Activate();
        else
            currency.Deactivate();

        await _currencies.UpdateAsync(currency, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<SetCurrencyStatusResponse>.Ok(
            new SetCurrencyStatusResponse(CurrencyDto.From(currency)));
    }

    public async Task<Result<ListCurrenciesResponse>> ListCurrenciesAsync(
        CancellationToken ct = default)
    {
        var list = await _currencies.ListAsync(ct);
        var response = new ListCurrenciesResponse(list.Select(CurrencyDto.From).ToList());

        return Result<ListCurrenciesResponse>.Ok(response);
    }
}