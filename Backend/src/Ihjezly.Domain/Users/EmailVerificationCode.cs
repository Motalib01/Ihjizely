using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users;

public sealed class EmailVerificationCode : Entity
{
    public string Email { get; private set; }
    public string Code { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    private EmailVerificationCode() { }

    private EmailVerificationCode(Guid id, string email, string code, DateTime expiresAt)
        : base(id)
    {
        Email = email;
        Code = code;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public static EmailVerificationCode Create(string email, string code, DateTime expiresAt)
        => new(Guid.NewGuid(), email, code, expiresAt);

    public bool IsValid()
        => !IsUsed && DateTime.UtcNow <= ExpiresAt;

    public void MarkAsUsed() => IsUsed = true;
}