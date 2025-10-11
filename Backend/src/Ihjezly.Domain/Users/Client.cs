using Ihjezly.Domain.Users.Events;

namespace Ihjezly.Domain.Users;

public sealed class Client : User
{
    private Client() : base(Guid.NewGuid()) { }

    private Client(string fullName, string phoneNumber, string email, string passwordHash)
        : base(fullName, phoneNumber, email, passwordHash, UserRole.Client)
    {
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, UserRole.Client));
    }

    public static Client Create(
        string fullName,
        string phoneNumber,
        string email,
        string passwordHash,
        bool isVerified = false)
    {
        var client = new Client(fullName, phoneNumber, email, passwordHash);
        client.SetIsVerified(isVerified);

        return client;
    }


}