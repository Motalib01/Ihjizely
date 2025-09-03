namespace Ihjezly.Application.Users.Common;

public sealed record CurrentUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role,
    bool IsVerified,
    string? ProfilePictureUrl
);