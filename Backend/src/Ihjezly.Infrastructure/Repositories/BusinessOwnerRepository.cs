using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class BusinessOwnerRepository : IBusinessOwnerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BusinessOwnerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(BusinessOwner businessOwner) => _dbContext.BusinessOwners.Add(businessOwner);

    public async Task<BusinessOwner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.BusinessOwners.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<BusinessOwner?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default) =>
        await _dbContext.BusinessOwners.FirstOrDefaultAsync(b => b.PhoneNumber == phoneNumber, cancellationToken);
}