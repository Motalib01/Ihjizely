namespace Ihjezly.Domain.Users.Repositories;

public interface IEmailVerificationRepository
{
    Task AddAsync(EmailVerificationCode code, CancellationToken cancellationToken);
    Task<EmailVerificationCode?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<EmailVerificationCode?> GetByUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken);
}