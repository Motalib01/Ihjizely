using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Events;

public sealed class Admin : User
{
    private Admin() : base(Guid.NewGuid()) { }

    private Admin(string fullName, string phoneNumber, string email, string passwordHash)
        : base(fullName, phoneNumber, email, passwordHash, UserRole.Admin)
    {
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, UserRole.Admin));
    }

    public static Admin Create(
        string fullName,
        string phoneNumber,
        string email,
        string passwordHash,
        bool isVerified = false)
    {
        var admin = new Admin(fullName,  phoneNumber, email, passwordHash);
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