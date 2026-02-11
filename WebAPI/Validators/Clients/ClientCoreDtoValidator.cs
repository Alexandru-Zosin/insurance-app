using Application.Services.Shared.DTOs.ClientDTOs;
using FluentValidation;
using WebAPI.Validators.Constants;
using WebAPI.Validators.Shared;

namespace WebAPI.Validators.Clients;

public sealed class ClientCoreDtoValidator : AbstractValidator<ClientCoreDto>
{
    public ClientCoreDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type is invalid.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(ClientValidationConstants.NameMaxLength)
            .WithMessage($"Name must be at most {ClientValidationConstants.NameMaxLength} characters.");

        RuleFor(x => x.IdentificationNumber)
            .NotNull()
            .WithMessage("IdentificationNumber is required.");

        When(x => x.IdentificationNumber is not null, () =>
        {
            RuleFor(x => x.IdentificationNumber).SetValidator(new IdentificationNumberDtoValidator());
        });

        RuleFor(x => x.ContactInfo)
            .NotNull()
            .WithMessage("ContactInfo is required.");

        When(x => x.ContactInfo is not null, () =>
        {
            RuleFor(x => x.ContactInfo).SetValidator(new ContactInfoDtoValidator());
        });

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!).SetValidator(new AddressDtoValidator());
        });
    }
}
