using Application.Services.Currencies.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Currencies;

public sealed class AddCurrencyRequestValidator : AbstractValidator<AddCurrencyRequest>
{
    public AddCurrencyRequestValidator()
    {
        RuleFor(x => x.Currency)
            .NotNull()
            .WithMessage("Currency is required.");

        When(x => x.Currency is not null, () =>
        {
            RuleFor(x => x.Currency).SetValidator(new CurrencyDtoValidator());
        });
    }
}
