namespace Ihjezly.Application.Users.Common;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }=string.Empty;
    public string Role { get; init; } = string.Empty;
    public string ProfilePictureUrl { get; init; } = string.Empty;
    public bool IsBlocked { get; init; } 
    public bool IsVerified { get; init; } 

}