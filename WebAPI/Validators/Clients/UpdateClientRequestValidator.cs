using Application.Services.Clients.DTOs;
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

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Number)
            .NotEmpty()
            .MaximumLength(20);
    }
}
