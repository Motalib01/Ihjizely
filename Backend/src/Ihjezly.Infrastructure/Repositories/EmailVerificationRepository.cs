using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class EmailVerificationRepository : IEmailVerificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmailVerificationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmailVerificationCode code, CancellationToken cancellationToken)
    {
        await _dbContext.Set<EmailVerificationCode>().AddAsync(code, cancellationToken);
    }

    public async Task<EmailVerificationCode?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<EmailVerificationCode>()
            .Where(x => x.UserId == userId && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.ExpiresAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmailVerificationCode?> GetByUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<EmailVerificationCode>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Code == code && !x.IsUsed, cancellationToken);
    }
}