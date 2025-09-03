using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Events;

public sealed class Admin : User
{
    private Admin() : base(Guid.NewGuid()) { }

    private Admin(string firstName, string lastName, string phoneNumber, string passwordHash)
        : base(firstName, lastName, phoneNumber, passwordHash, UserRole.Admin)
    {
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, UserRole.Admin));
    }

    public static Admin Create(
        string firstName,
        string lastName,
        string phoneNumber,
        string passwordHash,
        bool isVerified = false)
    {
        var admin = new Admin(firstName, lastName,  phoneNumber, passwordHash);
        admin.SetIsVerified(isVerified);
        return admin;
    }

    public void BlockUser(User user, string reason)
    {
        user.Block(reason);
    }

    public void UnblockUser(User user)
    {
        user.Unblock();
    }

}