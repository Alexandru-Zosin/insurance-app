using Application.Services.Currencies.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Currencies;

public sealed class SetCurrencyStatusRequestValidator : AbstractValidator<SetCurrencyStatusRequest>
{
    public SetCurrencyStatusRequestValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode is required.")
            .Length(CurrencyValidationConstants.CodeLength)
            .WithMessage($"CurrencyCode must be exactly {CurrencyValidationConstants.CodeLength} characters.")
            .Matches(CurrencyValidationConstants.CurrencyCodeRegex)
            .WithMessage("CurrencyCode format is invalid. Use 3 uppercase letters.");
    }
}
