using Application.Common;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Shared;

public sealed class PageRequestValidator : AbstractValidator<PageRequest>
{
    public PageRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(PagingValidationConstants.PageNumberMinValue)
            .WithMessage("Page(Number) must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(PagingValidationConstants.PageSizeMinValue, PagingValidationConstants.PageSizeMaxValue)
            .WithMessage($"PageSize must be between {PagingValidationConstants.PageSizeMinValue} and {PagingValidationConstants.PageSizeMaxValue}.");
    }
}
