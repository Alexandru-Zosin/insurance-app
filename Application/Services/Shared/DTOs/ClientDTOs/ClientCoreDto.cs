using Domain.Clients;

namespace Application.Services.Shared.DTOs.ClientDTOs;

public record ClientCoreDto(
    ClientType Type,
    string Name,
    IdentificationNumberDto IdentificationNumber,
    ContactInfoDto ContactInfo,  
    AddressDto? Address)
{
    public static ClientCoreDto From(Client e) =>
        new(
            e.Type,
            e.Name,
            IdentificationNumberDto.From(e.Identifier),
            ContactInfoDto.From(e.ContactInfo),
            AddressDto.From(e.Address));
}