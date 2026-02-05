namespace Application.Services.Currencies.DTOs
{
    public sealed record SetCurrencyStatusRequest(string CurrencyCode, bool Active);
}
