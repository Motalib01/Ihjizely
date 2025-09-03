using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Ihjezly.Infrastructure;
using Microsoft.EntityFrameworkCore;

internal sealed class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AdminRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Admin admin) => _dbContext.Users.Add(admin);

    public async Task<Admin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Users
            .OfType<Admin>()  // This filters by discriminator UserRole.Admin
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);


    public async Task<Admin?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default) =>
        await _dbContext.Users
            .OfType<Admin>()
            .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber, cancellationToken);
}