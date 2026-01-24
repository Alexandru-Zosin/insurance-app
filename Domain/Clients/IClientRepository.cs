namespace Domain.Clients;

public interface IClientRepository
{
    Client? GetById(Guid clientId);
    Client? GetByRegistrationNumber(string registrationNumber);
    void Add(Client client);
}
