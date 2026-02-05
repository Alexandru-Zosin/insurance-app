using Application.Services.Shared.DTOs;
using Domain.Shared;
using FluentValidation;

namespace WebAPI.Validators.Shared;

public sealed class RiskTagDtoValidator : AbstractValidator<RiskTagDto>
{
    public RiskTagDtoValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Category is invalid.");
    }
}
