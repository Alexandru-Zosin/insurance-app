using Application.Services.Shared.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Shared;

public sealed class IdentificationNumberDtoValidator : AbstractValidator<IdentificationNumberDto>
{
    public IdentificationNumberDtoValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required.")
            .MaximumLength(SharedVoValidationConstants.IdentificationNumberMaxLength)
            .WithMessage($"Value must be at most {SharedVoValidationConstants.IdentificationNumberMaxLength} characters.")
            .Matches(SharedVoValidationConstants.IdentificationNumberRegex)
            .WithMessage("Value format is invalid.");
    }
}
