using Application.Services.Currencies.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Currencies;

public sealed class UpdateCurrencyRequestValidator : AbstractValidator<UpdateCurrencyRequest>
{
    public UpdateCurrencyRequestValidator()
    {
        RuleFor(x => x.Currency)
            .NotNull()
            .WithMessage("Currency is required.");

        When(x => x.Currency is not null, () =>
        {
            RuleFor(x => x.Currency).SetValidator(new CurrencyUpdateDtoValidator());
        });
    }
}
