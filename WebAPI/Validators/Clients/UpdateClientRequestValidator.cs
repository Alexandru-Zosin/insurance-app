using Application.Services.Clients.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Clients;

public sealed class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleFor(x => x.ClientInfo)
            .NotNull()
            .WithMessage("ClientInfo is required.");

        When(x => x.ClientInfo is not null, () =>
        {
            RuleFor(x => x.ClientInfo).SetValidator(new ClientCoreDtoValidator());
        });
    }
}
