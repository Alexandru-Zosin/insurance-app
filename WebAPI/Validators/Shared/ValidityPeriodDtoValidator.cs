using Application.Services.Shared.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Shared;

public sealed class ValidityPeriodDtoValidator : AbstractValidator<ValidityPeriodDto>
{
    public ValidityPeriodDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("EndDate is required.");

        RuleFor(x => x)
            .Must(x => x.EndDate >= x.StartDate)
            .WithMessage("EndDate must be greater than or equal to StartDate.");
    }
}
