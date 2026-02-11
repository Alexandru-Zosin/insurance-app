using Application.Services.Buildings.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Buildings;

public sealed class RegisterBuildingRequestValidator : AbstractValidator<RegisterBuildingRequest>
{
    public RegisterBuildingRequestValidator()
    {
        RuleFor(x => x.Building)
            .NotNull()
            .WithMessage("Building is required.");

        When(x => x.Building is not null, () =>
        {
            RuleFor(x => x.Building).SetValidator(new BuildingCoreDtoValidator());
        });
    }
}
