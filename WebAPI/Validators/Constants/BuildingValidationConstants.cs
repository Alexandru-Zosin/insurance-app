namespace WebAPI.Validators.Constants;

public static class BuildingValidationConstants
{
    // BuildingCoreDto
    public const int CityIdMinValue = 1;

    public const int MinConstructionYear = 1600;

    public const int SurfaceAreaMinValue = 1;
    public const int SurfaceAreaMaxValue = 10_000_000;

    public const int RiskTagsMaxCount = 100;
}
