using Application.Services.Currencies.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Currencies;

public sealed class UpdateCurrencyRequestValidator : AbstractValidator<UpdateCurrencyRequest>
{
    public UpdateCurrencyRequestValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode is required.")
            .Length(CurrencyValidationConstants.CodeLength)
            .WithMessage($"CurrencyCode must be exactly {CurrencyValidationConstants.CodeLength} characters.")
            .Matches(CurrencyValidationConstants.CurrencyCodeRegex)
            .WithMessage("CurrencyCode format is invalid. Use 3 uppercase letters.");

        RuleFor(x => x.Currency)
            .NotNull()
            .WithMessage("Currency is required.");

        When(x => x.Currency is not null, () =>
        {
            RuleFor(x => x.Currency).SetValidator(new CurrencyUpdateDtoValidator());
        });
    }
}
