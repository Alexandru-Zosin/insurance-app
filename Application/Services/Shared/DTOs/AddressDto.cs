using Domain.Shared;

namespace Application.Services.Shared.DTOs;

public sealed record AddressDto(
 string Street,
 string Number)
{
    public static AddressDto? From(Address? v) =>
        v is null ? null : new(v.Street, v.Number);

    public Address ToDomain() => Address.Create(Street, Number);
}