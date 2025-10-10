using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Ihjezly.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<int> CountByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _context.Users.CountAsync(u => u.Role == role, cancellationToken);
    }

    public async Task<List<User>> GetAdminsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Admin)
            .ToListAsync(cancellationToken);
    }

    public void Update(User user) 
    {
        _context.Users.Update(user);
    }

    public async Task<User?> GetByPhoneOrEmailAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == identifier || u.Email == identifier, cancellationToken);
    }
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }


    public void Remove(User user) => _context.Users.Remove(user);

    public async Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is not null)
        {
            _context.Users.Remove(user);
        }
    }
}