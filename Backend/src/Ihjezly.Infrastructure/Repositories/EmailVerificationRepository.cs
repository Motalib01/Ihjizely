using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class EmailVerificationRepository : IEmailVerificationRepository
{
    private readonly ApplicationDbContext _context;

    public EmailVerificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EmailVerificationCode code, CancellationToken cancellationToken = default)
    {
        await _context.Set<EmailVerificationCode>().AddAsync(code, cancellationToken);
    }

    public void Update(EmailVerificationCode code)
    {
        _context.Set<EmailVerificationCode>().Update(code);
    }

    public async Task<EmailVerificationCode?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<EmailVerificationCode>()
            .OrderByDescending(e => e.ExpiresAt)
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<EmailVerificationCode?> GetActiveCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<EmailVerificationCode>()
            .Where(c => c.Email == email && !c.IsUsed && c.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(c => c.ExpiresAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}