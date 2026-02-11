using Application.Services.Reports.DTOs;
using Domain.Policies;
using FluentValidation;

namespace WebAPI.Validators.Reports;

public sealed class GetReportsCriteriaValidator : AbstractValidator<GetReportsCriteria>
{
    public GetReportsCriteriaValidator()
    {
        RuleFor(x => x.from)
            .NotEmpty();

        RuleFor(x => x.to)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.from);

        RuleFor(x => x.policyStatus)
            .Must(s => s is null || s != PolicyStatus.Draft)
            .WithMessage("policyStatus cannot be Draft.");

        When(x => x.currencyCode is not null, () =>
        {
            RuleFor(x => x.currencyCode!)
                .NotEmpty()
                .MaximumLength(4)
                .Matches("^[A-Z0-9]+$")
                .WithMessage("currencyCode must be uppercase alphanumeric.");
        });
    }
}
