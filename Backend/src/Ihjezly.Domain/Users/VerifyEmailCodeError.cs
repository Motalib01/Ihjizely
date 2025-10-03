using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users;

public static class VerifyEmailCodeError
{
    public static readonly Error NotFound = new(
        "User.VerifyEmailCode",
        "The VerifyEmailCode was not found.");

    public static Error Invalid = new(
        "User.Invalid",
        "Invalid or expired verification code.");
}