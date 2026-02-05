using Application.Services.Clients.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Clients;

public sealed class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Client)
            .NotNull()
            .WithMessage("Client is required.");

        When(x => x.Client is not null, () =>
        {
            RuleFor(x => x.Client).SetValidator(new ClientCoreDtoValidator());
        });
    }
}
