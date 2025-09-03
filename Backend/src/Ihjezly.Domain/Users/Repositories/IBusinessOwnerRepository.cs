namespace Ihjezly.Domain.Users.Repositories;

public interface IBusinessOwnerRepository
{
    Task<BusinessOwner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(BusinessOwner businessOwner);
    Task<BusinessOwner?> GetByPhoneNumberAsync(string requestPhoneNumber, CancellationToken cancellationToken);
}