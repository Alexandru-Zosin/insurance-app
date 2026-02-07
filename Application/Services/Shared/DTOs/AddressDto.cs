using Domain.Shared;

namespace Application.Services.Shared.DTOs;

public sealed record AddressDto(
 string Street,
 string Number)
{
    public static AddressDto From(Address v) => new(v.Street, v.Number);
    public Address ToDomain() => Address.Create(Street, Number);
}