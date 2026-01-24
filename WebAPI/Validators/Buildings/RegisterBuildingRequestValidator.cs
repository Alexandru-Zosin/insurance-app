using Application.UseCases.Buildings;
using Domain.Buildings;
using FluentValidation;

public sealed class RegisterBuildingRequestValidator
    : AbstractValidator<RegisterBuildingService.Request>
{
    public RegisterBuildingRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.CityId)
            .GreaterThan(0);

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Number)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.ConstructionYear)
            .InclusiveBetween(1500, DateTime.UtcNow.Year);

        RuleFor(x => x.BuildingType)
            .Must(v => Enum.TryParse<BuildingType>(v, out _))
            .WithMessage("Invalid building type");

        RuleFor(x => x.SurfaceArea)
            .GreaterThan(0);

        RuleFor(x => x.InsuredValue)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(10);
    }
}
