namespace Ihjezly.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneOrEmailAsync(string identifier, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default); 
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<List<User>> GetAdminsAsync(CancellationToken cancellationToken = default);
    void Update(User user);


    void Remove(User user);

    // Optional convenience: hard delete by id (loads then removes)
    Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default);

}

