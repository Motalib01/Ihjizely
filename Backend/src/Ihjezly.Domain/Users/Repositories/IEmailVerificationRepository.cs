namespace Ihjezly.Domain.Users.Repositories;

public interface IEmailVerificationRepository
{
    Task<EmailVerificationCode?> GetActiveCodeAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(EmailVerificationCode code, CancellationToken cancellationToken = default);
    void Update(EmailVerificationCode code);
    Task<EmailVerificationCode?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}