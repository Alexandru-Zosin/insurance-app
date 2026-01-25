using Application.Services.Clients.DTO;
using FluentValidation;

public sealed class UpdateClientRequestValidator
    : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Address)
            .MaximumLength(200);
    }
}
