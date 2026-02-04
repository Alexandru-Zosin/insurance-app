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

        await _currencies.AddAsync(currency, ct);

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
        UpdateCurrencyRequest request,
        CancellationToken ct = default)
    {
        var currency = await _currencies.GetByIdAsync(request.CurrencyId, ct);
        if (currency == null)
            return Result<UpdateCurrencyResponse>.Fail(
                ErrorType.NotFound, "Currency not found");

        currency.UpdateExchangeRateToBase(request.Currency.ExchangeRateToBase);

        await _currencies.UpdateAsync(currency, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateCurrencyResponse>.Ok(
            new UpdateCurrencyResponse(CurrencyDto.From(currency)));
    }

    public async Task<Result<SetCurrencyStatusResponse>> SetCurrencyStatusAsync(
        SetCurrencyStatusRequest request,
        CancellationToken ct = default)
    {
        var currency = await _currencies.GetByIdAsync(request.CurrencyId, ct);
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
        ListCurrenciesRequest request,
        CancellationToken ct = default)
    {
        var list = await _currencies.ListAsync(request.OnlyActive, ct);
        var response = new ListCurrenciesResponse(list.Select(CurrencyDto.From).ToList());

        return Result<ListCurrenciesResponse>.Ok(response);
    }
}