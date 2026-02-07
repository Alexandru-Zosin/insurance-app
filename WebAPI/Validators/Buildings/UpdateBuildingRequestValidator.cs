using Application.Services.Buildings.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Buildings;

public sealed class UpdateBuildingRequestValidator : AbstractValidator<UpdateBuildingRequest>
{
    public UpdateBuildingRequestValidator()
    {
        RuleFor(x => x.BuildingId)
            .NotEmpty()
            .WithMessage("BuildingId is required.");

        RuleFor(x => x.BuildingInfo)
            .NotNull()
            .WithMessage("BuildingInfo is required.");

        When(x => x.BuildingInfo is not null, () =>
        {
            RuleFor(x => x.BuildingInfo).SetValidator(new BuildingCoreDtoValidator());
        });
    }
}