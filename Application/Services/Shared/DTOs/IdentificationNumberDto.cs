using Domain.Shared;
namespace Application.Services.Shared.DTOs;

public sealed record IdentificationNumberDto(string Value)
{
    public static IdentificationNumberDto? From(IdentificationNumber? v) =>
        v is null ? null : new(v.Value);
    
    public IdentificationNumber ToDomain() => IdentificationNumber.Create(Value);
}
