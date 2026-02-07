using Application.Services.Shared.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Shared;

public sealed class MoneyDtoValidator : AbstractValidator<MoneyDto>
{
    public MoneyDtoValidator()
    {
        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Amount is required.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode is required.")
            .Length(SharedVoValidationConstants.CurrencyCodeLength)
            .WithMessage($"CurrencyCode must be exactly {SharedVoValidationConstants.CurrencyCodeLength} characters.")
            .Matches(SharedVoValidationConstants.CurrencyCodeRegex)
            .WithMessage("CurrencyCode format is invalid. Use ISO-like 3-letter uppercase code.");
    }
}
