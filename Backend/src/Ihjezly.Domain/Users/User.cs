using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Users.Events;

namespace Ihjezly.Domain.Users;

public abstract class User : Entity
{
    protected User(Guid id) : base(id) { }

    public string FirstName { get; protected set; } = string.Empty;
    public string LastName { get; protected set; } = string.Empty;
    public string? PhoneNumber { get; protected set; } = string.Empty;
    public string? Email { get; protected set; } = string.Empty;
    public string Password { get; protected set; } = string.Empty;
    public UserRole Role { get; protected set; }
    public Image UserProfilePicture { get; protected set; } = Image.Default;
    public bool IsVerified { get; private set; } = false;

    // Track violations or bad behavior
    public int ViolationCount { get; private set; } = 0;

    // Admin-controlled blocking
    public bool IsBlocked { get; private set; } = false;
    public DateTime? BlockedAt { get; private set; }

    protected User(
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string password,
        UserRole role)
        : this(Guid.NewGuid())
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Password = password;
        Role = role;
        UserProfilePicture = Image.Default;
    }

    public void SetIsVerified(bool isVerified)
    {
        IsVerified = isVerified;
    }

    public void MarkPhoneAsVerified()
    {
        SetIsVerified(true);
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber, string email, Image userProfilePicture)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        UserProfilePicture = userProfilePicture;

        RaiseDomainEvent(new UserUpdatedDomainEvent(Id));
    }

    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
        RaiseDomainEvent(new UserPasswordChangedDomainEvent(Id));
    }

    public void Delete()
    {
        RaiseDomainEvent(new UserDeletedDomainEvent(Id));
    }

    public void UpdateProfilePicture(Image newProfilePicture)
    {
        if (newProfilePicture is null)
            throw new ArgumentNullException(nameof(newProfilePicture));

        UserProfilePicture = newProfilePicture;
        RaiseDomainEvent(new UserProfilePictureUpdatedDomainEvent(Id, newProfilePicture.Url));
    }

    // Called when a user breaks a rule
    public void ReportViolation()
    {
        ViolationCount++;
    }

    public void ResetViolations()
    {
        ViolationCount = 0;
    }

    // Admin-controlled
    public void Block(string reason)
    {
        if (IsBlocked) return;

        IsBlocked = true;
        BlockedAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserBlockedDomainEvent(Id, reason));
    }

    public void Unblock()
    {
        if (!IsBlocked) return;

        IsBlocked = false;
        BlockedAt = null;
        RaiseDomainEvent(new UserUnblockedDomainEvent(Id));
    }

    public bool CanLogin()
    {
        return !IsBlocked;
    }
}
