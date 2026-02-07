using Application.Common;
using Application.Services.Currencies;
using Application.Services.Currencies.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/currencies")]
public sealed class CurrenciesController(ICurrencyService CurrencyService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListCurrenciesResponse>> List(
        CancellationToken ct = default)
    {
        var result = await CurrencyService.ListCurrenciesAsync(ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<AddCurrencyResponse>> Add(
        [FromBody] AddCurrencyRequest request,
        CancellationToken ct)
    {
        var result = await CurrencyService.AddCurrencyAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/currencies/{result.Value!.Currency.Code}");
    }

    [HttpPut("{currencyCode}")]
    public async Task<ActionResult<UpdateCurrencyResponse>> Update(
        [FromRoute] string currencyCode,
        [FromBody] UpdateCurrencyRequest request,
        CancellationToken ct)
    {
        var result = await CurrencyService.UpdateCurrencyAsync(currencyCode, request, ct);

        return FromResult(result);
    }

    [HttpPut("{currencyCode}/status")]
    public async Task<ActionResult<SetCurrencyStatusResponse>> SetStatus(
        [FromRoute] string currencyCode,
        [FromBody] SetCurrencyStatusRequest request,
        CancellationToken ct)
    {
        var result = await CurrencyService.SetCurrencyStatusAsync(currencyCode, request, ct);

        return FromResult(result);
    }
}
