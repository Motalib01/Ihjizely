using Ihjezly.Domain.Users;

namespace Ihjezly.Application.Users.Common;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Role = user.Role.ToString(),
            ProfilePictureUrl = user.UserProfilePicture.Url,
            IsVerified = user.IsVerified,
            IsBlocked = user.IsBlocked,
        };
    }
}