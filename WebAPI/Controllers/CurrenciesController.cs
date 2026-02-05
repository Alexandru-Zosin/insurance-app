using Application.Common;
using Application.Services.Currencies;
using Application.Services.Currencies.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

namespace WebAPI.Controllers;

[Route("api/admin/currencies")]
public sealed class CurrenciesController(ICurrencyService CurrencyService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListCurrenciesResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var pagedRequest = new PageRequest(page, pageSize);
        var result = await CurrencyService.ListCurrenciesAsync(
            new ListCurrenciesRequest(pagedRequest), ct);

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

    [HttpPut("{currencyCode:string}")]
    public async Task<ActionResult<UpdateCurrencyResponse>> Update(
        [FromBody] UpdateCurrencyRequest request,
        CancellationToken ct)
    {
        var result = await CurrencyService.UpdateCurrencyAsync(request, ct);

        return FromResult(result);
    }

    [HttpPut("{currencyCode:string}/status")]
    public async Task<ActionResult<SetCurrencyStatusResponse>> SetStatus(
        [FromBody] SetCurrencyStatusRequest request,
        CancellationToken ct)
    {
        var result = await CurrencyService.SetCurrencyStatusAsync(request, ct);

        return FromResult(result);
    }
}
