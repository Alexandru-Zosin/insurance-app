using Domain.Clients;

namespace Infrastructure.Persistence.Repositories
{
    public interface IClientRepository
    {
        void Add(Client client);
        Client? GetById(Guid clientId);
        Client? GetByRegistrationNumber(string registrationNumber);
        IReadOnlyList<Client> SearchByName(string name);
        void Update(Client client);
    }
}