using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.Common;
using Ihjezly.Domain.Users;

namespace Ihjezly.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? Email,
    string Password,
    UserRole Role) : ICommand<UserDto>;