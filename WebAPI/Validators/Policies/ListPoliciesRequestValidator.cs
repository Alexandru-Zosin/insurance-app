using Application.Services.Policies.DTOs;
using FluentValidation;
using WebAPI.Validators.Shared;

namespace WebAPI.Validators.Policies;

public sealed class ListPoliciesRequestValidator : AbstractValidator<ListPoliciesRequest>
{
    public ListPoliciesRequestValidator()
    {
        RuleFor(x => x.Page)
            .NotNull()
            .WithMessage("Page is required.");

        When(x => x.Page is not null, () =>
        {
            RuleFor(x => x.Page).SetValidator(new PageRequestValidator());
        });

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status!.Value)
                .IsInEnum()
                .WithMessage("Status is invalid.");
        });

        RuleFor(x => x.ClientId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("ClientId must be a non-empty GUID when provided.");

        RuleFor(x => x.BrokerId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("BrokerId must be a non-empty GUID when provided.");

        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.EndDate!.Value >= x.StartDate!.Value)
                .WithMessage("EndDate must be greater than or equal to StartDate.");
        });
    }
}
