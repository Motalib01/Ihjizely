using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Events;

public sealed class BusinessOwner : User
{
    private BusinessOwner() : base(Guid.NewGuid()) { }

    private BusinessOwner(string firstName, string lastName, string phoneNumber, string email, string passwordHash)
        : base(firstName, lastName, phoneNumber, email, passwordHash, UserRole.BusinessOwner)
    {
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, UserRole.BusinessOwner));
    }

    public static BusinessOwner Create(
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string passwordHash,
        bool isVerified = false)
    {
        var owner = new BusinessOwner(firstName, lastName, phoneNumber, email, passwordHash);
        owner.SetIsVerified(isVerified);
        return owner;
    }
}