using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Ihjezly.Infrastructure;
using Microsoft.EntityFrameworkCore;

internal sealed class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ClientRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Client client) => _dbContext.Users.Add(client); // ✅ Add via Users DbSet

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Users
            .OfType<Client>()                         // ✅ Filter by type
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);


    public async Task<Client?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default) =>
        await _dbContext.Users
            .OfType<Client>()                         // ✅ Filter by type
            .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber, cancellationToken);
}