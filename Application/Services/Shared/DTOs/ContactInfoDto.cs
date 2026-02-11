using Domain.Shared;

namespace Application.Services.Shared.DTOs;

public sealed record ContactInfoDto(string Email, string Phone)
{
    public static ContactInfoDto From(ContactInfo v) => new(v.Email, v.Phone);

    public ContactInfo MapToDomain() => ContactInfo.Create(Email, Phone);

}
