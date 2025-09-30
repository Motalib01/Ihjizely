using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users;

public sealed class EmailVerificationCode : Entity
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    private EmailVerificationCode() { }

    private EmailVerificationCode(Guid id, Guid userId, string code, DateTime expiresAt)
        : base(id)
    {
        UserId = userId;
        Code = code;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public static EmailVerificationCode Create(Guid userId, string code, DateTime expiresAt)
    {
        return new EmailVerificationCode(Guid.NewGuid(), userId, code, expiresAt);
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}