using Ihjezly.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Ihjezly.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    IFormFile? ProfileImageFile
) : ICommand;