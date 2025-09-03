using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;