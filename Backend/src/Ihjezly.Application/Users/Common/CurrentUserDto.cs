namespace Ihjezly.Application.Users.Common;

public sealed record CurrentUserDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string Role,
    bool IsVerified,
    string? ProfilePictureUrl
);