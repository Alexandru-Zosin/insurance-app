using WebAPI.Validators.Constants;
using Application.Services.Shared.DTOs.BrokerDTOs;
using FluentValidation;
using WebAPI.Validators.Shared;

namespace WebAPI.Validators.Brokers;

public sealed class BrokerCoreDtoValidator : AbstractValidator<BrokerCoreDto>
{
    public BrokerCoreDtoValidator()
    {
        RuleFor(x => x.BrokerCode)
            .NotEmpty().WithMessage("BrokerCode is required.")
            .MaximumLength(BrokerValidationConstants.BrokerCodeMaxLength)
            .WithMessage($"BrokerCode must be at most {BrokerValidationConstants.BrokerCodeMaxLength} characters.")
            .Matches(BrokerValidationConstants.BrokerCodeRegex)
            .WithMessage("BrokerCode contains invalid characters. Use A-Z, 0-9, '_' or '-'.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(BrokerValidationConstants.BrokerNameMaxLength)
            .WithMessage($"Name must be at most {BrokerValidationConstants.BrokerNameMaxLength} characters.");

        RuleFor(x => x.ContactInfo)
            .NotNull()
            .WithMessage("ContactInfo is required.");

        When(x => x.ContactInfo is not null, () =>
        {
            RuleFor(x => x.ContactInfo).SetValidator(new ContactInfoDtoValidator());
        });

        RuleFor(x => x.CommissionPercentage)
            .InclusiveBetween(0.0m, 1.0m)
            .WithMessage("CommissionPercentage must be between 0.0 and 1.0.")
            .When(x => x.CommissionPercentage.HasValue);
    }
}
