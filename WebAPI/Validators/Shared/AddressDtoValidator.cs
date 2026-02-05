using Application.Services.Shared.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Shared;

public sealed class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(SharedVoValidationConstants.StreetMaxLength)
            .WithMessage($"Street must be at most {SharedVoValidationConstants.StreetMaxLength} characters.");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Number is required.")
            .MaximumLength(SharedVoValidationConstants.NumberMaxLength)
            .WithMessage($"Number must be at most {SharedVoValidationConstants.NumberMaxLength} characters.")
            .Matches(SharedVoValidationConstants.AddressNumberRegex)
            .WithMessage("Number format is invalid.");
    }
}
