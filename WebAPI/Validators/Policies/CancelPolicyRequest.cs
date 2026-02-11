using Application.Services.Policies.DTOs;
using FluentValidation;
using WebAPI.Validators.Constants;

namespace WebAPI.Validators.Policies;

public sealed class CancelPolicyRequestValidator : AbstractValidator<CancelPolicyRequest>
{
    public CancelPolicyRequestValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty()
            .WithMessage("BrokerId is required.");

        RuleFor(x => x.CancellationEffectiveDate)
            .NotEmpty()
            .WithMessage("CancellationEffectiveDate is required.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(PolicyValidationConstants.CancellationReasonMaxLength)
            .WithMessage($"Reason must be at most {PolicyValidationConstants.CancellationReasonMaxLength} characters.");
    }
}
