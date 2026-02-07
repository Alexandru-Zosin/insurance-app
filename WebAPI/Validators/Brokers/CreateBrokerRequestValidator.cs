using WebAPI.Validators.Constants;
using Application.Services.Brokers.DTOs;
using FluentValidation;
namespace WebAPI.Validators.Brokers;

public sealed class CreateBrokerRequestValidator : AbstractValidator<CreateBrokerRequest>
{
    public CreateBrokerRequestValidator()
    {
        RuleFor(x => x.Broker)
            .NotNull()
            .WithMessage("Broker is required.");

        When(x => x.Broker is not null, () =>
        {
            RuleFor(x => x.Broker).SetValidator(new BrokerCoreDtoValidator());
        });
    }
}
