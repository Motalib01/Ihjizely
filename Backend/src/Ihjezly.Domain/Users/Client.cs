using Ihjezly.Domain.Users.Events;

namespace Ihjezly.Domain.Users;

public sealed class Client : User
{
    private Client() : base(Guid.NewGuid()) { }

    private Client(string firstName, string lastName, string phoneNumber, string passwordHash)
        : base(firstName, lastName, phoneNumber, passwordHash, UserRole.Client)
    {
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, UserRole.Client));
    }

    public static Client Create(
        string firstName,
        string lastName,
        string phoneNumber,
        string passwordHash,
        bool isVerified = false)
    {
        var client = new Client(firstName, lastName, phoneNumber, passwordHash);
        client.SetIsVerified(isVerified);

        return client;
    }


}