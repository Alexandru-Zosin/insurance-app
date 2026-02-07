using Application.Services.Shared.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Shared;

public sealed class ContactInfoDtoValidator : AbstractValidator<ContactInfoDto>
{
    public ContactInfoDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(SharedVoValidationConstants.EmailMaxLength)
            .WithMessage($"Email must be at most {SharedVoValidationConstants.EmailMaxLength} characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .MaximumLength(SharedVoValidationConstants.PhoneMaxLength)
            .WithMessage($"Phone must be at most {SharedVoValidationConstants.PhoneMaxLength} characters.")
            .Matches(SharedVoValidationConstants.PhoneRegex)
            .WithMessage("Phone format is invalid.");
    }
}
