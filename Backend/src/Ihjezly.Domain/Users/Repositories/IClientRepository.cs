namespace Ihjezly.Domain.Users.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Client?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    void Add(Client client);
}