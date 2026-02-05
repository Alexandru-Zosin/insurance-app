namespace Application.Services.Currencies.DTOs
{
    public sealed record SetCurrencyStatusRequest(Guid CurrencyCode, bool Active);
}
