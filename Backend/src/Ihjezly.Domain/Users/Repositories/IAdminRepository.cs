namespace Ihjezly.Domain.Users.Repositories;

public interface IAdminRepository
{
    Task<Admin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Admin admin);
    Task<Admin?> GetByPhoneNumberAsync(string requestPhoneNumber, CancellationToken cancellationToken);
}