using Application.Services.Brokers.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Brokers;

public sealed class UpdateBrokerRequestValidator : AbstractValidator<UpdateBrokerRequest>
{
    public UpdateBrokerRequestValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty()
            .WithMessage("BrokerId is required.");

        RuleFor(x => x.Broker)
            .NotNull()
            .WithMessage("Broker is required.");

        When(x => x.Broker is not null, () =>
        {
            RuleFor(x => x.Broker).SetValidator(new BrokerCoreDtoValidator());
        });
    }
}
