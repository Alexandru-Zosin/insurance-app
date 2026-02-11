using Application.Services.Shared.DTOs.PolicyDTOs;
using FluentValidation;
using WebAPI.Validators.Constants;
using WebAPI.Validators.Shared;

namespace WebAPI.Validators.Policies;

public sealed class PolicyCoreDtoValidator : AbstractValidator<PolicyCoreDto>
{
    public PolicyCoreDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("ClientId is required.");

        RuleFor(x => x.BuildingId)
            .NotEmpty()
            .WithMessage("BuildingId is required.");

        RuleFor(x => x.BrokerId)
            .NotEmpty()
            .WithMessage("BrokerId is required.");

        RuleFor(x => x.Tenure)
            .NotNull()
            .WithMessage("Tenure is required.");

        When(x => x.Tenure is not null, () =>
        {
            RuleFor(x => x.Tenure).SetValidator(new ValidityPeriodDtoValidator());
        });

        RuleFor(x => x.BasePremium)
            .NotNull()
            .WithMessage("BasePremium is required.");

        When(x => x.BasePremium is not null, () =>
        {
            RuleFor(x => x.BasePremium).SetValidator(new MoneyDtoValidator());
        });

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode is required.")
            .Length(CurrencyValidationConstants.CodeLength)
            .WithMessage($"CurrencyCode must be exactly {CurrencyValidationConstants.CodeLength} characters.")
            .Matches(CurrencyValidationConstants.CurrencyCodeRegex)
            .WithMessage("CurrencyCode format is invalid. Use 3 uppercase letters.");
    }
}
