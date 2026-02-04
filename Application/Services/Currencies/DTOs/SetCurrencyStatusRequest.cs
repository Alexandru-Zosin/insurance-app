namespace Application.Services.Currencies.DTOs
{
    public sealed record SetCurrencyStatusRequest(Guid CurrencyId, bool Active);
}
