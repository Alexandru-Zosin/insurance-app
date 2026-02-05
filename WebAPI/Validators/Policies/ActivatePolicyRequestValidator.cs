using Application.Services.Policies.DTOs;
using FluentValidation;

namespace WebAPI.Validators.Policies;

public sealed class ActivatePolicyRequestValidator : AbstractValidator<ActivatePolicyRequest>
{
    public ActivatePolicyRequestValidator()
    {
        RuleFor(x => x.BrokerId)
            .NotEmpty()
            .WithMessage("BrokerId is required.");

        RuleFor(x => x.PolicyNumber)
            .NotEmpty()
            .WithMessage("PolicyNumber is required.");
    }
}
