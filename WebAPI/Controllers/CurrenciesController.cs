using Application.Services.Currencies;
using Application.Services.Currencies.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/currencies")]
public sealed class CurrenciesController(ICurrencyService currencyService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListCurrenciesResponse>> ListCurrenciesAsync(CancellationToken ct = default)
    {
        var result = await currencyService.ListCurrenciesAsync(ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<AddCurrencyResponse>> AddCurrencyAsync(
        [FromBody] AddCurrencyRequest request,
        CancellationToken ct = default)
    {
        var result = await currencyService.AddCurrencyAsync(request, ct);

        return FromCreated(result, $"/api/admin/currencies/{result.Value!.Currency.Code}");
    }

    [HttpPut("{currencyCode}")]
    public async Task<ActionResult<UpdateCurrencyResponse>> UpdateCurrencyAsync(
        [FromRoute] string currencyCode,
        [FromBody] UpdateCurrencyRequest request,
        CancellationToken ct = default)
    {
        var result = await currencyService.UpdateCurrencyAsync(currencyCode, request, ct);

        return FromResult(result);
    }

    [HttpPost("{currencyCode}/activate")]
    public async Task<ActionResult<SetCurrencyStatusResponse>> ActivateCurrencyAsync(
    [FromRoute] string currencyCode,
    CancellationToken ct = default)
    {
        var result = await currencyService.SetCurrencyStatusAsync(currencyCode, true, ct);

        return FromResult(result);
    }

    [HttpPost("{currencyCode}/deactivate")]
    public async Task<ActionResult<SetCurrencyStatusResponse>> DeactivateCurrencyAsync(
        [FromRoute] string currencyCode,
        CancellationToken ct = default)
    {
        var result = await currencyService.SetCurrencyStatusAsync(currencyCode, false, ct);

        return FromResult(result);
    }
}
