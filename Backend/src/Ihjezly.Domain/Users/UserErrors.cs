using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound", 
        "The user was not found.");

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials", 
        "Invalid Phone Number or password.");

    public static readonly Error PhoneNumberAlreadyInUse = new(
        "User.PhoneNumberAlreadyInUse",
        "The phone number is already registered.");

    public static Error InvalidPassword = new(
        "User.InvalidPassword",
        "The old password is incorrect.");

    public static Error PasswordConfirmationMismatch = new(
        "User.PasswordConfirmationMismatch",
        "The new password and confirmation do not match.");

    public static Error UserBlocked = new(
        "User.Blocked",
        "This user is blocked by an admin and cannot log in");

    public static Error CannotDeleteWithBalance = new(
        "User.CannotDeleteWithBalance",
        "Can not Delet is have balance in his wallet");
}