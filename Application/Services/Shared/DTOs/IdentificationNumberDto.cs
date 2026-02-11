using Domain.Shared;
namespace Application.Services.Shared.DTOs;

public sealed record IdentificationNumberDto(string Value)
{
    public static IdentificationNumberDto From(IdentificationNumber v) => new(v.Value);
    
    public IdentificationNumber MapToDomain() => IdentificationNumber.Create(Value);
}
