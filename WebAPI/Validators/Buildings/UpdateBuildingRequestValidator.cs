using Application.Services.Buildings.DTO;
using FluentValidation;

public sealed class UpdateBuildingRequestValidator
    : AbstractValidator<UpdateBuildingRequest>
{
    public UpdateBuildingRequestValidator()
    {
        RuleFor(x => x.BuildingId)
            .NotEmpty();

        RuleFor(x => x.ConstructionYear)
            .InclusiveBetween(1500, DateTime.UtcNow.Year);

        RuleFor(x => x.SurfaceArea)
            .GreaterThan(0);

        RuleFor(x => x.InsuredValue)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(10);
    }
}
