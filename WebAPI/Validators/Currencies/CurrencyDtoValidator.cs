using Application.Services.Shared.DTOs.CurrencyDTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Currencies;

public sealed class CurrencyDtoValidator : AbstractValidator<CurrencyDto>
{
    public CurrencyDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .Length(CurrencyValidationConstants.CodeLength)
            .WithMessage($"Code must be exactly {CurrencyValidationConstants.CodeLength} characters.")
            .Matches(CurrencyValidationConstants.CurrencyCodeRegex)
            .WithMessage("Code format is invalid. Use 3 uppercase letters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(CurrencyValidationConstants.NameMaxLength)
            .WithMessage($"Name must be at most {CurrencyValidationConstants.NameMaxLength} characters.");

        RuleFor(x => x.ExchangeRateToBase)
            .GreaterThan(CurrencyValidationConstants.ExchangeRateMinExclusive)
            .WithMessage("ExchangeRateToBase must be greater than 0.")
            .LessThanOrEqualTo(CurrencyValidationConstants.ExchangeRateMaxInclusive)
            .WithMessage($"ExchangeRateToBase must be less than or equal to {CurrencyValidationConstants.ExchangeRateMaxInclusive}.");
    }
}
