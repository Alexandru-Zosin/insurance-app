using Application.UseCases.Clients;
using FluentValidation;

public sealed class CreateClientRequestValidator
    : AbstractValidator<CreateClientService.Request>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.ClientType)
            .NotEmpty()
            .Must(v => v == "Individual" || v == "Company");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(50);

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
