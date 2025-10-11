using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Events;

public sealed class BusinessOwner : User
{
    private BusinessOwner() : base(Guid.NewGuid()) { }

    private BusinessOwner(string fullName, string phoneNumber, string email, string passwordHash)
        : base(fullName, phoneNumber, email, passwordHash, UserRole.BusinessOwner)
    {
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, UserRole.BusinessOwner));
    }

    public static BusinessOwner Create(
        string fullName,
        string phoneNumber,
        string email,
        string passwordHash,
        bool isVerified = false)
    {
        var owner = new BusinessOwner(fullName, phoneNumber, email, passwordHash);
        owner.SetIsVerified(isVerified);
        return owner;
    }
}