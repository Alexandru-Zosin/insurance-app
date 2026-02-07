using Application.Services.Shared.DTOs.BuildingDTOs;
using FluentValidation;
using WebAPI.Validators.Constants;
using WebAPI.Validators.Shared;

namespace WebAPI.Validators.Buildings;

public sealed class BuildingCoreDtoValidator : AbstractValidator<BuildingCoreDto>
{
    public BuildingCoreDtoValidator()
    {
        RuleFor(x => x.OwnerClientId)
            .NotEmpty()
            .WithMessage("OwnerClientId is required.");

        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Address is required.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address).SetValidator(new AddressDtoValidator());
        });

        RuleFor(x => x.CityId)
            .GreaterThanOrEqualTo(BuildingValidationConstants.CityIdMinValue)
            .WithMessage("CityId must be a positive integer.");

        RuleFor(x => x.ConstructionYear)
            .InclusiveBetween(BuildingValidationConstants.MinConstructionYear, DateTime.UtcNow.Year)
            .WithMessage($"ConstructionYear must be between {BuildingValidationConstants.MinConstructionYear} and the current year.");

        RuleFor(x => x.BuildingType)
            .IsInEnum()
            .WithMessage("BuildingType is invalid.");

        RuleFor(x => x.SurfaceArea)
            .InclusiveBetween(BuildingValidationConstants.SurfaceAreaMinValue, BuildingValidationConstants.SurfaceAreaMaxValue)
            .WithMessage($"SurfaceArea must be between {BuildingValidationConstants.SurfaceAreaMinValue} and {BuildingValidationConstants.SurfaceAreaMaxValue}.");

        RuleFor(x => x.InsuredValue)
            .NotNull()
            .WithMessage("InsuredValue is required.");

        When(x => x.InsuredValue is not null, () =>
        {
            RuleFor(x => x.InsuredValue).SetValidator(new MoneyDtoValidator());
        });

        RuleFor(x => x.ZoneRiskCategories)
            .NotNull()
            .WithMessage("ZoneRiskCategories is required.")
            .Must(list => list.Count <= BuildingValidationConstants.RiskTagsMaxCount)
            .WithMessage($"ZoneRiskCategories must contain at most {BuildingValidationConstants.RiskTagsMaxCount} items.")
            .Must(list => list.Distinct().Count() == list.Count)
            .WithMessage("ZoneRiskCategories must not contain duplicates.");

        When(x => x.ZoneRiskCategories is not null, () =>
        {
            RuleForEach(x => x.ZoneRiskCategories)
                .IsInEnum()
                .WithMessage("ZoneRiskCategories contains an invalid value.");
        });
    }
}
